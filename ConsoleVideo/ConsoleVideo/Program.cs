//TODO: Either just remove CUDA support or just rewrite this whole application in C++.

using ConsoleVideo.IO;
using ConsoleVideo.Math;
using ConsoleVideo.Media;
using ConsoleVideo.Windows;
using FFMediaToolkit;
using FFMediaToolkit.Decoding;
using FFMediaToolkit.Graphics;
using Microsoft.WindowsAPICodePack.Dialogs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ConsoleVideo {
    internal static class Program {
        private static bool IsPlatformSupported => (Environment.OSVersion.Platform == PlatformID.Win32NT);
        private static readonly Vector2Int fontPixelSize = new(8, 8);

        private static void Main(string[] args) {
            try {
                ExitCode exitCode = Run(args);

                Console.Write("Execution done.\r\n" +
                              $"Exit code: {exitCode}.\r\n");
                Console.ReadKey();
                Environment.Exit((int)(exitCode));
            } catch (Exception exception) {
                Console.Write($"{exception.Message}\r\n" + $"{exception.InnerException}\r\n" + $"{exception.StackTrace}");
                Console.ReadKey();
            }
            return;
        }

        private static ExitCode Run(IReadOnlyList<string> arguments) {
            if (IsPlatformSupported == false) {
                return ExitCode.PlatformNotSupported;
            }

            Console.Title = "Console video.";
            Console.CursorVisible = false;
            Console.Clear();

            //Yes, this is a shitty way of handling errors.
            ExitCode tempExitCode = InitializeFFmpeg(arguments.Contains("--automaticFFmpeg")); //TODO: THIS IS A BAD WAY OF HANDLING ARGUMENTS, WRITE A BETTER CODE.
            if (tempExitCode != ExitCode.Success) {
                return tempExitCode;
            }

            tempExitCode = LoadVideo(out Video video);
            if (tempExitCode != ExitCode.Success) {
                return tempExitCode;
            }

            /*
                The scale looks weird when resized because the vertical size of the font is larger than it's width.
                To prevent that, we switch the font to "Raster Font" and set the size to 8 x 8.
            */
            tempExitCode = SwitchFont();
            if (tempExitCode != ExitCode.Success) {
                return tempExitCode;
            }

            tempExitCode = DisableResize();
            if (tempExitCode != ExitCode.Success) {
                return tempExitCode;
            }

            Vector2Int windowSize = ResizeConsole(video);
            Console.Write($"Console size: ({(windowSize.x * fontPixelSize.x)}, {(windowSize.y * fontPixelSize.y)}).\r\n");

            IEnumerable<IFrame> frames = GenerateFrames(windowSize,
                                                        video,
                                                        video.resolution);

            //Just in case.
            GC.Collect(2,
                       GCCollectionMode.Forced,
                       true,
                       true);
            GC.WaitForPendingFinalizers();

            bool stable = UserInput.AskUserChoice('s',
                                                  "Stable wait time (It might get faster or slower than the original).",
                                                  "Variable wait time (It might get choppy).");

            double sleepTime = (1000d / video.frameRate),
                   previousMilliseconds = sleepTime;
            Stopwatch stopwatch = new();
            foreach (IFrame frame in frames) {
                stopwatch.Restart();

                Console.SetCursorPosition(0, 0);
                for (int y = 0; y < windowSize.y; ++y) {
                    for (int x = 0; x < windowSize.x; ++x) {
                        FastConsole.Write(frame.GetPixel(y, x)
                                               .ToString());
                    }
                }
                FastConsole.Flush();

                while (true) {
                    double milliseconds = stopwatch.Elapsed.TotalMilliseconds;
                    bool condition = ((stable == true) ? (milliseconds >= sleepTime) : (milliseconds >= (sleepTime + (sleepTime - previousMilliseconds))));
                    if (condition == true) {
                        previousMilliseconds = milliseconds;
                        break;
                    }
                }
            }
            return ExitCode.Success;
        }

        private static ExitCode InitializeFFmpeg(bool automaticSearch) {
            if (automaticSearch == true) {
                FFmpegLoader.FFmpegPath = IoUtilities.FindDirectory("ffmpeg");
            } else {
                using CommonOpenFileDialog commonOpenFileDialog = new("Select an FFmpeg folder.") {
                    InitialDirectory = FilePath.ApplicationDirectory,
                    IsFolderPicker = true
                };

                if (commonOpenFileDialog.ShowDialog() != CommonFileDialogResult.Ok) {
                    return ExitCode.FileDialogFailed;
                }

                FFmpegLoader.FFmpegPath = commonOpenFileDialog.FileName;
            }

            FFmpegLoader.LoadFFmpeg();
            return ExitCode.Success;
        }

        private static ExitCode LoadVideo(out Video video) {
            video = null;

            using CommonOpenFileDialog commonOpenFileDialog = new("Select a video to play.");
            commonOpenFileDialog.Filters.Add(new CommonFileDialogFilter("mp4 video", "*.mp4"));
            commonOpenFileDialog.Filters.Add(new CommonFileDialogFilter("AVI video", "*.avi"));

            if (commonOpenFileDialog.ShowDialog() != CommonFileDialogResult.Ok) {
                return ExitCode.FileDialogFailed;
            }

            string videoFilePath = commonOpenFileDialog.FileName;

            video = new Video(MediaFile.Open(videoFilePath));
            return ExitCode.Success;
        }

        private static ExitCode SwitchFont() {
            IntPtr outputHandle = WindowsApi.GetStdHandle(ParameterConstant.STD_OUTPUT_HANDLE);
            if (outputHandle == WindowsApi.INVALID_HANDLE_VALUE) {
                Console.Error.Write("Failed to get output handle.\r\n");
                return ExitCode.FailedToModifyConsoleWindow;
            }

            CONSOLE_FONT_INFOEX newFont = new() {
                dwFontSize = new COORD((short)(fontPixelSize.x), (short)(fontPixelSize.y)),
                FontFamily = 0,
                FontWeight = FontWeights.Normal
            };
            newFont.cbSize = (uint)(Marshal.SizeOf(newFont));
            return ConsoleConfiguration.SwitchFont(outputHandle,
                                                   newFont,
                                                   "Terminal");
        }

        private static ExitCode DisableResize() {
            IntPtr consoleHandle = WindowsApi.GetConsoleWindow();
            if (consoleHandle == WindowsApi.INVALID_HANDLE_VALUE) {
                Console.Error.Write("Failed to get the console handle.\r\n");
                return ExitCode.FailedToModifyConsoleWindow;
            }
            return ConsoleConfiguration.DisableResize(consoleHandle);
        }

        private static Vector2Int ResizeConsole(Video video) {
            //const int emptySpace = 5;
            const int emptySpace = 0;
            Vector2Int largestWindowSize = new((Console.LargestWindowWidth - emptySpace), (Console.LargestWindowHeight - emptySpace)),
                       windowSize = new(largestWindowSize.x, largestWindowSize.y);

            if (largestWindowSize.x > largestWindowSize.y) {
                windowSize.x = (int)(largestWindowSize.y * video.ratio);
            } else {
                windowSize.y = (int)(largestWindowSize.x * video.ratio);
            }
            Console.SetWindowSize(windowSize.x, windowSize.y);
            Console.SetBufferSize(windowSize.x, (windowSize.y + 1));
            return windowSize;
        }

        private static IEnumerable<IFrame> GenerateFrames(Vector2Int windowSize,
                                                          Video video,
                                                          Vector2Int videoSize) {
            bool cuda = UserInput.AskUserChoice('c',
                                                "Use CUDA 11.4 (Very slow due to bad implementation + dependency of .NET libraries. Memory leak may occur).",
                                                $"Use {nameof(Parallel)}.{nameof(Parallel.For)}.");

            FrameGenerator frameGenerator = new(cuda,
                                                windowSize,
                                                video.resolution.x,
                                                video.resolution.y);

            IList<IFrame> frames = new List<IFrame>();
            int frameCount = 0;
            while (video.mediaFile.Video.TryGetNextFrame(out ImageData imageData) == true) {
                using (Image<Rgb24> image = Image.LoadPixelData<Rgb24>(imageData.Data,
                                                                       videoSize.x,
                                                                       videoSize.y)) {
                    frames.Add(frameGenerator.Convert(image));
                    image.Dispose();
                }
                Console.Write($"\r{++frameCount} frames converted.");
            }
            video.mediaFile.Dispose();
            Console.Write("\r\n");
            return frames.ToArray();
        }
    }
}