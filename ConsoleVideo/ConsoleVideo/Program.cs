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

namespace ConsoleVideo;

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

    private static ExitCode Run(IEnumerable<string> arguments) {
        if (IsPlatformSupported == false) {
            return ExitCode.PlatformNotSupported;
        }

        Console.Title = "Console video.";
        Console.CursorVisible = false;
        Console.Clear();

        ExitCode tempExitCode = InitializeFFmpeg(arguments.Contains("--automaticFFmpeg"));
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
        using CommonOpenFileDialog commonOpenFileDialog = new("Select a video to play.");
        commonOpenFileDialog.Filters.Add(new CommonFileDialogFilter("mp4 video", "*.mp4"));
        commonOpenFileDialog.Filters.Add(new CommonFileDialogFilter("AVI video", "*.avi"));

        if (commonOpenFileDialog.ShowDialog() != CommonFileDialogResult.Ok) {
            video = null;
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
        Vector2Int largestWindowSize = new(Console.LargestWindowWidth, Console.LargestWindowHeight),
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
        bool gpu = UserInput.AskUserChoice('g',
                                           "Use GPU.",
                                           $"Use {nameof(Parallel)}.{nameof(Parallel.For)}.");

        Console.Write("\r\nPress E to stop converting frames and start replaying.\r\n");

        IFrameGenerator frameGenerator;
        if (gpu) {
            frameGenerator = new GpuFrameGenerator(windowSize, video.resolution);
        } else {
            frameGenerator = new FrameGenerator(windowSize, video.resolution);
        }

        IList<IFrame> frames = new List<IFrame>();
        int frameCount = 0;

        while (video.mediaFile.Video.TryGetNextFrame(out ImageData imageData) == true) {
            using Image<Bgr24> image = Image.LoadPixelData<Bgr24>(imageData.Data,
                                                                  videoSize.x,
                                                                  videoSize.y);
            frames.Add(frameGenerator.Convert(image));
            Console.Write($"\r{++frameCount} frames converted.");

            if (Console.KeyAvailable &&
                Console.ReadKey()
                       .Key ==
                ConsoleKey.E) {
                break;
            }
        }

        video.mediaFile.Dispose();
        Console.Write("\r\n");
        return frames.ToArray();
    }
}

/*
    CPU normal: avg: 1083.8
    1071.
    1100.
    1081.
    1090.
    1078.
    
    CPU ACCEL: avg: 6440.2
    6447.
    6446.
    6392.
    6443.
    6473.
    
    GPU ACCEL: avg: 3735
    3826.
    3653.
    3852
    3659.
    3685.
    
    GPU ACCEL OPTIMIZATION ATTEMPT 1: avg: 3497.33333333 //An improvement!
    3504.
    3478.
    3510.
    
    GPU ACCEL OPTIMIZATION ATTEMPT 2: avg: 3501.33333333 //Worse? No effect? Better? Needs more testing.
    3565.
    3402.
    3537.
    
    GPU ACCEL OPTIMIZATION ATTEMPT 3: avg: 3509.66666667 //Same thing as before, we don't know for sure.
    3403.
    3535.
    3591.
    
    GPU ACCEL OPTIMIZATION ATTEMPT 4: avg: 3193.66666667 //A drastic improvement!
    3161.
    3250.
    3170.

    GPU ACCEL OPTIMIZATION ATTEMPT 5: avg: 3196 //Same, maybe the compiler took care of it already?
    3178.
    3239.
    3171.
    
    GPU ACCEL OPTIMIZATION ATTEMPT 6: avg: 3165.66666667 //Minor improvement.
    3140.
    3157.
    3200.
*/