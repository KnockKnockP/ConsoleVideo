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
using System.Linq;
using System.Text;
using System.Xml;

namespace ConsoleVideo {
    internal enum VariableType {
        Global,
        Player
    }

    internal class Variable {
        internal string Name {
            get;
        }

        internal string Id {
            get;
        }

        internal VariableType VariableType {
            get;
        }

        internal Variable(string name,
                          string id,
                          VariableType variableType) =>
            (Name, Id, VariableType) = (name, id, variableType);
    }

    internal static class Variables {
        internal static class Global {
            internal static Variable bots = new("bots", "#4ZmAI-vJd!l](jlWUzx", VariableType.Global);
        }

        internal static class Player {
            internal static Variable xPos = new("XPos", "5H=*|N/9GDv7.m2xeB#h", VariableType.Player);
            internal static Variable zPos = new("ZPos", "eLIiC!~J-hc/g0swP|yZ", VariableType.Player);
            internal static Variable originalXPos = new("OriginalXPos", "t+bpytFC@2U_67K_GR~$", VariableType.Player);
            internal static Variable originalZPos = new("OriginalZPos", "?6M.DyoFB8!)KKs4w/R0", VariableType.Player);
        }
    }

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

            const string filePath = @"C:\Users\memeb\Desktop\xml.xml";

            /*
            XmlDocument xmlDocument = new();
            
            XmlNode subroutineBlock = xmlDocument.CreateElement("root");
            xmlDocument.AppendChild(subroutineBlock);

            subroutineBlock.AppendChild(GenerateVariableReference(xmlDocument,
                                                                  Variables.Player.originalXPos,
                                                                  null));

            xmlDocument.Save(filePath);
            */

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
            XmlDocument xmlDocument = new();

            {
                //Subroutine block.
                XmlNode subroutineBlock = xmlDocument.CreateElement("block", "https://developers.google.com/blocky/xml");
                xmlDocument.AppendChild(subroutineBlock);

                {
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
                }

                {
                    //Subroutine name.
                    XmlNode subroutineName = xmlDocument.CreateElement("field");
                    subroutineName.InnerText = "PlayVideo";
                    subroutineBlock.AppendChild(subroutineName);

                    //Subroutine name.name.
                    XmlAttribute subroutineNameName = xmlDocument.CreateAttribute("name");
                    subroutineNameName.Value = "SUBROUTINE_NAME";
                    subroutineName.Attributes?.Append(subroutineNameName);
                }

                {
                    //Actions block.
                    XmlNode actionsBlock = xmlDocument.CreateElement("statement");
                    subroutineBlock.AppendChild(actionsBlock);

                    {
                        //Actions.name.
                        XmlAttribute actionsName = xmlDocument.CreateAttribute("name");
                        actionsName.Value = "ACTIONS";
                        actionsBlock.Attributes?.Append(actionsName);
                    }

                    //SetVariable.
                    actionsBlock.AppendChild(GenerateSetVariable(xmlDocument,
                                                                 Variables.Player.xPos,
                                                                 0,
                                                                 false));
                }
            }

            XmlWriterSettings xmlWriterSettings = new() {
                Async = false,
                CheckCharacters = true,
                CloseOutput = true,
                ConformanceLevel = ConformanceLevel.Document,
                DoNotEscapeUriAttributes = true,
                Encoding = Encoding.UTF8,
                Indent = true,
                IndentChars = "    ",
                NamespaceHandling = NamespaceHandling.Default,
                NewLineChars = "\r\n"
            };
            XmlWriter xmlWriter = XmlWriter.Create(filePath, xmlWriterSettings);
            xmlDocument.Save(xmlWriter);
            return;
        }

        private static XmlNode GenerateSetVariable(XmlDocument xmlDocument,
                                                   Variable variable,
                                                   int botsIndex,
                                                   bool pixelActivated) {
            //SetVariable.
            XmlNode setVariable = xmlDocument.CreateElement("block");

            {
                //SetVariable.name.
                XmlAttribute setVariableType = xmlDocument.CreateAttribute("type");
                setVariableType.Value = "SetVariable";
                setVariable.Attributes?.Append(setVariableType);
            }

            {
                //SetVariable first parameter.
                XmlNode setVariableFirstParameter = xmlDocument.CreateElement("value");
                setVariable.AppendChild(setVariableFirstParameter);

                {
                    //SetVariable first parameter.name.
                    XmlAttribute setVariableFirstParameterName = xmlDocument.CreateAttribute("name");
                    setVariableFirstParameterName.Value = "VALUE-0";
                    setVariableFirstParameter.Attributes?.Append(setVariableFirstParameterName);
                }

                setVariableFirstParameter.AppendChild(GenerateVariableReference(xmlDocument,
                                                                                Variables.Player.xPos,
                                                                                null));
            }

            return setVariable;
        }

        private static XmlNode GenerateVariableReference(XmlDocument xmlDocument,
                                                         Variable variable,
                                                         XmlNode objectInstance) {
            //VariableReferenceBlock
            XmlNode variableReferenceBlock = xmlDocument.CreateElement("block");

            {
                //VariableReferenceBlock.type.
                XmlAttribute variableReferenceBlockType = xmlDocument.CreateAttribute("type");
                variableReferenceBlockType.Value = "variableReferenceBlock";
                variableReferenceBlock.Attributes?.Append(variableReferenceBlockType);
            }

            bool isObjectVar = (variable.VariableType != VariableType.Global);
            {
                //VariableReferenceBlock.mutation
                XmlNode variableReferenceBlockMutation = xmlDocument.CreateElement("mutation");
                variableReferenceBlock.AppendChild(variableReferenceBlockMutation);

                {
                    //VariableReferenceBlock.mutation.isObjectVar
                    XmlAttribute variableReferenceBlockBlockMutationIsObjectVar = xmlDocument.CreateAttribute("isObjectVar");
                    variableReferenceBlockBlockMutationIsObjectVar.Value = isObjectVar.ToString()
                                                                                      .ToLower();
                    variableReferenceBlockMutation.Attributes?.Append(variableReferenceBlockBlockMutationIsObjectVar);
                }
            }

            {
                //VariableReferenceBlock.objectType.
                XmlNode variableReferenceBlockObjectType = xmlDocument.CreateElement("field");
                variableReferenceBlockObjectType.InnerText = variable.VariableType.ToString();
                variableReferenceBlock.AppendChild(variableReferenceBlockObjectType);

                {
                    //VariableReferenceBlock.objectType.name.
                    XmlAttribute variableReferenceBlockObjectTypeName = xmlDocument.CreateAttribute("name");
                    variableReferenceBlockObjectTypeName.Value = "OBJECTTYPE";
                    variableReferenceBlockObjectType.Attributes?.Append(variableReferenceBlockObjectTypeName);
                }
            }

            {
                //VariableReferenceBlock.var.
                XmlNode variableReferenceBlockVar = xmlDocument.CreateElement("field");
                variableReferenceBlockVar.InnerText = variable.Name;
                variableReferenceBlock.AppendChild(variableReferenceBlockVar);

                {
                    //VariableReferenceBlock.var.name.
                    XmlAttribute variableReferenceBlockVarName = xmlDocument.CreateAttribute("name");
                    variableReferenceBlockVarName.Value = "VAR";
                    variableReferenceBlockVar.Attributes?.Append(variableReferenceBlockVarName);

                    //VariableReferenceBlock.var.id.
                    XmlAttribute variableReferenceBlockVarId = xmlDocument.CreateAttribute("id");
                    variableReferenceBlockVarId.Value = variable.Id;
                    variableReferenceBlockVar.Attributes?.Append(variableReferenceBlockVarId);

                    //VariableReferenceBlock.var.variableType.
                    XmlAttribute variableReferenceBlockVarVariableType = xmlDocument.CreateAttribute("variableType");
                    variableReferenceBlockVarVariableType.Value = variable.VariableType.ToString();
                    variableReferenceBlockVar.Attributes?.Append(variableReferenceBlockVarVariableType);
                }
            }

            return variableReferenceBlock;
        }
    }
}