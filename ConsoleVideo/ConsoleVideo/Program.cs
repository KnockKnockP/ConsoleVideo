using ConsoleVideo.IO;
using ConsoleVideo.Math;
using ConsoleVideo.Media;
using FFMediaToolkit;
using FFMediaToolkit.Decoding;
using FFMediaToolkit.Graphics;
using Microsoft.WindowsAPICodePack.Dialogs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace ConsoleVideo {
    internal static class Program {
        private static void Main() {
            try {
                ExitCode exitCode = Run();

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

        private static ExitCode Run() {
            InitializeFFmpeg(true);
            LoadVideo(out Video video);

            IEnumerable<IFrame> frames = GenerateFrames(new Vector2Int(4, 4),
                                                        video,
                                                        video.resolution);

            const string filePath = @"C:\Users\memeb\Desktop\xml.txt";
            GenerateXml(frames.ToArray(), filePath);
            return ExitCode.Success;
        }

        private static void InitializeFFmpeg(bool automaticSearch) {
            if (automaticSearch == true) {
                FFmpegLoader.FFmpegPath = IoUtilities.FindDirectory("ffmpeg");
            } else {
                using CommonOpenFileDialog commonOpenFileDialog = new("Select an FFmpeg folder.") {
                    InitialDirectory = FilePath.ApplicationDirectory,
                    IsFolderPicker = true
                };

                if (commonOpenFileDialog.ShowDialog() != CommonFileDialogResult.Ok) {
                    return;
                }

                FFmpegLoader.FFmpegPath = commonOpenFileDialog.FileName;
            }

            FFmpegLoader.LoadFFmpeg();
            return;
        }

        private static void LoadVideo(out Video video) {
            video = null;

            using CommonOpenFileDialog commonOpenFileDialog = new("Select a video to play.");
            commonOpenFileDialog.Filters.Add(new CommonFileDialogFilter("mp4 video", "*.mp4"));
            commonOpenFileDialog.Filters.Add(new CommonFileDialogFilter("AVI video", "*.avi"));

            if (commonOpenFileDialog.ShowDialog() != CommonFileDialogResult.Ok) {
                return;
            }

            string videoFilePath = commonOpenFileDialog.FileName;

            video = new Video(MediaFile.Open(videoFilePath));
            return;
        }

        private static IEnumerable<IFrame> GenerateFrames(Vector2Int windowSize,
                                                          Video video,
                                                          Vector2Int videoSize) {
            FrameGenerator frameGenerator = new(windowSize,
                                                video.resolution.x,
                                                video.resolution.y);

            IList<IFrame> frames = new List<IFrame>();
            int totalFrameCount = 0,
                frameCount = 0;
            while (video.mediaFile.Video.TryGetNextFrame(out ImageData imageData) == true) {
                ++frameCount;
                switch (frameCount) {
                    case 1: {
                        using (Image<Rgb24> image = Image.LoadPixelData<Rgb24>(imageData.Data,
                                                                               videoSize.x,
                                                                               videoSize.y)) {
                            frames.Add(frameGenerator.Convert(image));
                            image.Dispose();
                        }
                        Console.Write($"\r{++totalFrameCount} frames converted to convertable format.");
                        break;
                    }
                    case 30:
                        frameCount = 0;
                        break;
                }
            }
            video.mediaFile.Dispose();
            Console.Write("\r\n");
            return frames.ToArray();
        }

        private static void GenerateXml(ICollection<IFrame> frames, string filePath) {
            /*
            StringBuilder stringBuilder = new("<block xmlns=\"https://developers.google.com/blocky/xml\" type=\"subroutineBlock\" deletable=\"false\" x=\"0\" y=\"3000\"\r\n" +
                                              "<field name=\"SUBROUTINE_NAME\">PlayVideo</field>\r\n" +
                                              "<statement name=\"ACTIONS\">\r\n" +
                                              "<block type=\"SetVariable\">\r\n" + //SetVariable start.
                                              "<value name=\"VALUE-0\">\r\n" + //SetVariable first parameter start.
                                              "<block type=\"variableReferenceBlock\">\r\n" + //SetVariable.VariableReference start.
                                              "<mutation isObjectVar=\"true\" />\r\n" +
                                              "<field name=\"OBJECTTYPE\">Player</field>\r\n" +
                                              "");
            */
            /*
            StringBuilder stringBuilder = new("<block xmlns=\"https://developers.google.com/blocky/xml\" type=\"subroutineBlock\" deletable=\"false\" x=\"0\" y=\"3000\"\r\n" +
                                              "<field name=\"SUBROUTINE_NAME\">PlayVideo</field>\r\n" +
                                              "<statement name=\"ACTIONS\">\r\n");

            for (int i = 0; i < frames.Count; ++i) {
                for (int y = 0; y < 4; ++y) {
                    for (int x = 0; x < 4; ++x) {
                        if (i != 0) {
                            stringBuilder.Append("<next>");
                        }
                    }
                }
            }
            stringBuilder.Append("</statement>\r\n" +
                                 "</block>");
            return stringBuilder.ToString();
            */

            XmlDocument xmlDocument = new();

            {
                //Subroutine block.
                XmlNode subroutineBlock = xmlDocument.CreateElement("block");

                //Subroutine.xmlns.
                XmlAttribute subroutineXmlns = xmlDocument.CreateAttribute("xmlns");
                subroutineXmlns.Value = "https://developers.google.com/blocky/xml";
                subroutineBlock.Attributes?.Append(subroutineXmlns);

                //Subroutine.type.
                XmlAttribute subroutineType = xmlDocument.CreateAttribute("type");
                subroutineType.Value = "subroutineBlock";
                subroutineBlock.Attributes?.Append(subroutineType);

                //Subroutine.deletable.
                XmlAttribute subroutineDeletable = xmlDocument.CreateAttribute("deletable");
                subroutineDeletable.Value = "false";
                subroutineBlock.Attributes?.Append(subroutineDeletable);

                //Subroutine.x.
                XmlAttribute subroutineX = xmlDocument.CreateAttribute("x");
                subroutineX.Value = "0";
                subroutineBlock.Attributes?.Append(subroutineX);

                //Subroutine.y.
                XmlAttribute subroutineY = xmlDocument.CreateAttribute("y");
                subroutineY.Value = "3000";
                subroutineBlock.Attributes?.Append(subroutineY);

                xmlDocument.AppendChild(subroutineBlock);
            }

            xmlDocument.Save(filePath);
            return;
        }
    }
}