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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace ConsoleVideo {
    internal static class Program {
        private static readonly Vector2Int size = new(7, 7);
        private const bool inverted = true;

        private static void Main() {
            try {
                ExitCode exitCode = Run();

                Console.Write("Execution done.\r\n" +
                              $"Exit code: {exitCode}.\r\n");
                Console.ReadLine();
                Environment.Exit((int)(exitCode));
            } catch (Exception exception) {
                Console.Write($"{exception.Message}\r\n" + $"{exception.InnerException}\r\n" + $"{exception.StackTrace}");
                Console.ReadLine();
            }
            return;
        }

        private static ExitCode Run() {
            InitializeFFmpeg(true);
            LoadVideo(out Video video);

            IList<IFrame<char>> frames = GenerateFrames(size,
                                                        video,
                                                        video.resolution);

            /*
                We have about 100kb of space to work with.
             
                Total frames: 219.
                
                Done list:
                    //First 100.
                    0 ~ 10. Done.
                    11 ~ 15. Done.
                    16 ~ 21. Done.
                    22 ~ 27. Done.
                    28 ~ 35. Done.
                    36 ~ 40. Done.
            */
            const int startFrameInclusive = 36,
                      endFrameInclusive = 40;
            CharFrame baseFrame = (CharFrame)(frames[(frames.Count - 1)]);

            const string filePath = @"C:\Users\memeb\Desktop\xml.xml";
            GenerateXml(frames,
                        filePath,
                        baseFrame,
                        startFrameInclusive,
                        endFrameInclusive);
            PlayVideo(frames,
                      startFrameInclusive,
                      endFrameInclusive);
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
            //video = new Video(MediaFile.Open(@"D:\GitHub\ConsoleVideo\ConsoleVideo\ConsoleVideo\Resources\Videos\Shape Video.mp4"));
            video = new Video(MediaFile.Open(@"D:\GitHub\ConsoleVideo\ConsoleVideo\ConsoleVideo\Resources\Videos\Original.mp4"));
            return;
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

        private static IList<IFrame<char>> GenerateFrames(Vector2Int windowSize,
                                                          Video video,
                                                          Vector2Int videoSize) {
            FrameGenerator frameGenerator = new(windowSize,
                                                video.resolution.x,
                                                video.resolution.y,
                                                inverted);

            IList<IFrame<char>> frames = new List<IFrame<char>>();

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
            
            frames.Add(new CharFrame(videoSize));
            for (int i = 0; i < (videoSize.x * videoSize.y); ++i) {
                char deactivatedChar = ' ';
                if (inverted) {
                    deactivatedChar = '#';
                }

                frames[(frames.Count - 1)]
                    .SetPixel(i, deactivatedChar);
            }
            return frames;
        }

        private static void PlayVideo(IList<IFrame<char>> frames,
                                      int startFrameInclusive,
                                      int endFrameInclusive) {
            Console.ReadLine();
            Console.Clear();

            //double sleepTime = 1000d,
            //       previousMilliseconds = sleepTime;
            //Stopwatch stopwatch = new();
            for (int i = startFrameInclusive; i <= endFrameInclusive; ++i) {
                IFrame<char> frame = frames[i];
                //stopwatch.Restart();

                Console.SetCursorPosition(0, 0);
                for (int y = 0; y < size.x; ++y) {
                    for (int x = 0; x < size.y; ++x) {
                        char charToWrite = frame.GetPixel(y, x);
                        if (charToWrite == ' ') {
                            charToWrite = '.';
                        }

                        FastConsole.Write(charToWrite
                                              .ToString());
                    }
                    FastConsole.Write("\r\n");
                }
                FastConsole.Write(i.ToString());
                FastConsole.Flush();

                Console.ReadLine();
                /*
                while (true) {
                    double milliseconds = stopwatch.Elapsed.TotalMilliseconds;
                    bool condition = (milliseconds >= (sleepTime + (sleepTime - previousMilliseconds)));
                    if (condition == true) {
                        previousMilliseconds = milliseconds;
                        break;
                    }
                }
                */
            }
            return;
        }

        #region Xml Generation.
        private static void GenerateXml(IList<IFrame<char>> frames,
                                        string filePath,
                                        CharFrame baseFrame,
                                        int startFrameInclusive,
                                        int endFrameInclusive) {
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

                    {
                        XmlNode previousBlock = null;

                        for (int i = startFrameInclusive; i <= endFrameInclusive; ++i) {
                            CharFrame frame = (CharFrame)(frames[i]);

                            CharFrame previousFrame;
                            if (i == startFrameInclusive) {
                                previousFrame = baseFrame;
                            } else {
                                previousFrame = (CharFrame)(frames[(i - 1)]);
                            }
                            for (int j = 0; j < (frame.Size.x * frame.Size.y); ++j) {
                                if (frame.GetPixel(j) != previousFrame.GetPixel(j)) {
                                    Console.Write($"Generating pixel for frame {i} pixel {j}.\r\n");

                                    bool activated = (frame.GetPixel(j) == '#');

                                    XmlNode setVariable = GenerateSetVariable(xmlDocument,
                                                                              Variables.Player.xPos,
                                                                              j,
                                                                              activated);

                                    if (previousBlock == null) {
                                        actionsBlock.AppendChild(setVariable);
                                    } else {
                                        XmlNode nextBlock = GenerateNextBlock(xmlDocument);
                                        previousBlock.AppendChild(nextBlock);
                                        nextBlock.AppendChild(setVariable);
                                    }
                                    previousBlock = setVariable;
                                }
                            }

                            XmlNode sleepSubroutineBlock = GenerateSubroutineInstanceBlock(xmlDocument, "Sleep"),
                                    nextBlockForSleep = GenerateNextBlock(xmlDocument);
                            previousBlock?.AppendChild(nextBlockForSleep);
                            nextBlockForSleep.AppendChild(sleepSubroutineBlock);

                            previousBlock = sleepSubroutineBlock;
                        }
                        XmlNode sleepBlocker = GenerateClearAllCustomMessages(xmlDocument),
                                sleepBlockerNext = GenerateNextBlock(xmlDocument);

                        previousBlock?.AppendChild(sleepBlockerNext);
                        sleepBlockerNext.AppendChild(sleepBlocker);
                    }
                }
            }

            if (File.Exists(filePath)) {
                File.Delete(filePath);
            }

            XmlWriterSettings xmlWriterSettings = new() {
                Async = false,
                CheckCharacters = true,
                CloseOutput = true,
                ConformanceLevel = ConformanceLevel.Auto,
                DoNotEscapeUriAttributes = true,
                Encoding = Encoding.UTF8,
                Indent = false,
                IndentChars = "",
                NamespaceHandling = NamespaceHandling.OmitDuplicates,
                NewLineChars = "\n"
            };

            using XmlWriter xmlWriter = XmlWriter.Create(filePath, xmlWriterSettings);
            xmlDocument.Save(xmlWriter);

            xmlWriter.Close();
            xmlWriter.Dispose();

            FileInfo fileInfo = new(filePath);
            Console.Write($"File size: {(fileInfo.Length * 0.001)} kilobytes.\r\n");
            return;
        }
        
        private static XmlNode GenerateClearAllCustomMessages(XmlDocument xmlDocument) {
            //ClearAllCustomMessages.
            XmlNode clearAllCustomMessages = xmlDocument.CreateElement("block");

            {
                //ClearAllCustomMessages.type.
                XmlAttribute clearAllCustomMessagesType = xmlDocument.CreateAttribute("type");
                clearAllCustomMessagesType.Value = "ClearAllCustomNotificationMessages";
                clearAllCustomMessages.Attributes?.Append(clearAllCustomMessagesType);
            }

            return clearAllCustomMessages;
        }

        private static XmlNode GenerateNextBlock(XmlDocument xmlDocument) => xmlDocument.CreateElement("next");

        private static XmlNode GenerateGetVariable(XmlDocument xmlDocument,
                                                   Variable variable,
                                                   XmlNode objectInstance = null) {
            //GetVariable.
            XmlNode getVariable = xmlDocument.CreateElement("block");

            {
                //GetVariable.type.
                XmlAttribute getVariableName = xmlDocument.CreateAttribute("type");
                getVariableName.Value = "GetVariable";
                getVariable.Attributes?.Append(getVariableName);
            }

            {
                //GetVariable first parameter.
                XmlNode getVariableFirstParameter = xmlDocument.CreateElement("value");
                getVariable.AppendChild(getVariableFirstParameter);

                {
                    //GetVariable first parameter.name.
                    XmlAttribute getVariableFirstParameterName = xmlDocument.CreateAttribute("name");
                    getVariableFirstParameterName.Value = "VALUE-0";
                    getVariableFirstParameter.Attributes?.Append(getVariableFirstParameterName);
                }

                {
                    VariableReferenceBlock variableReferenceBlock = new(xmlDocument, variable, objectInstance);
                    getVariableFirstParameter.AppendChild(variableReferenceBlock.GeneratedVariableReferenceBlock);
                }
            }

            return getVariable;
        }

        private static XmlNode GenerateValueInArray(XmlDocument xmlDocument,
                                                    Variable variable,
                                                    XmlNode objectInstance,
                                                    int index) {
            //ValueInArray.
            XmlNode valueInArray = xmlDocument.CreateElement("block");

            {
                //ValueInArray.type.
                XmlAttribute valueInArrayType = xmlDocument.CreateAttribute("type");
                valueInArrayType.Value = "ValueInArray";
                valueInArray.Attributes?.Append(valueInArrayType);
            }

            {
                //ValueInArray first parameter.
                XmlNode valueInArrayFirstParameter = xmlDocument.CreateElement("value");
                valueInArray.AppendChild(valueInArrayFirstParameter);

                {
                    //ValueInArray first parameter.name.
                    XmlAttribute valueInArrayFirstParameterName = xmlDocument.CreateAttribute("name");
                    valueInArrayFirstParameterName.Value = "VALUE-0";
                    valueInArrayFirstParameter.Attributes?.Append(valueInArrayFirstParameterName);
                }

                {
                    XmlNode parameterContent = GenerateGetVariable(xmlDocument,
                                                                   variable,
                                                                   objectInstance);
                    valueInArrayFirstParameter.AppendChild(parameterContent);
                }
            }

            {
                //ValueInArray second parameter.
                XmlNode valueInArraySecondParameter = xmlDocument.CreateElement("value");
                valueInArray.AppendChild(valueInArraySecondParameter);

                {
                    //ValueInArray second parameter.name.
                    XmlAttribute valueInArraySecondParameterName = xmlDocument.CreateAttribute("name");
                    valueInArraySecondParameterName.Value = "VALUE-1";
                    valueInArraySecondParameter.Attributes?.Append(valueInArraySecondParameterName);
                }

                {
                    XmlNode parameterContent = GenerateNumberLiteral(xmlDocument, index);
                    valueInArraySecondParameter.AppendChild(parameterContent);
                }
            }

            return valueInArray;
        }

        private static XmlNode GenerateNumberLiteral(XmlDocument xmlDocument, int number) {
            //NumberLiteral.
            XmlNode numberLiteral = xmlDocument.CreateElement("block");

            {
                //NumberLiteral.type.
                XmlAttribute numberLiteralType = xmlDocument.CreateAttribute("type");
                numberLiteralType.Value = "Number";
                numberLiteral.Attributes?.Append(numberLiteralType);
            }

            {
                //NumberLiteral.num.
                XmlNode numberLiteralNum = xmlDocument.CreateElement("field");
                numberLiteralNum.InnerText = number.ToString();
                numberLiteral.AppendChild(numberLiteralNum);

                {
                    //NumberLiteral.num.name.
                    XmlAttribute numberLiteralNumName = xmlDocument.CreateAttribute("name");
                    numberLiteralNumName.Value = "NUM";
                    numberLiteralNum.Attributes?.Append(numberLiteralNumName);
                }
            }

            return numberLiteral;
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

                {
                    XmlNode valueInArray = GenerateValueInArray(xmlDocument,
                                                                Variables.Global.bots,
                                                                null,
                                                                botsIndex);

                    VariableReferenceBlock variableReferenceBlock = new(xmlDocument, variable, valueInArray);
                    setVariableFirstParameter.AppendChild(variableReferenceBlock.GeneratedVariableReferenceBlock);
                }
            }

            {
                //SetVariable second parameter.
                XmlNode setVariableSecondParameter = xmlDocument.CreateElement("value");
                setVariable.AppendChild(setVariableSecondParameter);

                {
                    //SetVariable second parameter.name.
                    XmlAttribute setVariableSecondParameterName = xmlDocument.CreateAttribute("name");
                    setVariableSecondParameterName.Value = "VALUE-1";
                    setVariableSecondParameter.Attributes?.Append(setVariableSecondParameterName);
                }

                {
                    /*
                        Activated = white.
                        White = 0.
                        Black = original position.
                    */
                    XmlNode parameterContent;
                    if (pixelActivated == true) {
                        parameterContent = GenerateNumberLiteral(xmlDocument, 0);
                    } else {
                        parameterContent = GenerateGetVariable(xmlDocument,
                                                               Variables.Player.originalXPos,
                                                               GenerateValueInArray(xmlDocument,
                                                                                    Variables.Global.bots,
                                                                                    null,
                                                                                    botsIndex)
                                                              );
                    }

                    setVariableSecondParameter.AppendChild(parameterContent);
                }
            }

            return setVariable;
        }

        private static XmlNode GenerateSubroutineInstanceBlock(XmlDocument xmlDocument, string subroutineName) {
            //SubroutineInstanceBlock.
            XmlNode subroutineInstanceBlock = xmlDocument.CreateElement("block");

            {
                //SubroutineInstanceBlock.type.
                XmlAttribute subroutineInstanceBlockType = xmlDocument.CreateAttribute("type");
                subroutineInstanceBlockType.Value = "subroutineInstanceBlock";
                subroutineInstanceBlock.Attributes?.Append(subroutineInstanceBlockType);
            }

            {
                //SubroutineInstanceBlock.subroutineName.
                XmlNode subroutineInstanceBlockSubroutineName = xmlDocument.CreateElement("field");
                subroutineInstanceBlockSubroutineName.InnerText = subroutineName;
                subroutineInstanceBlock.AppendChild(subroutineInstanceBlockSubroutineName);

                {
                    //SubroutineInstanceBlock.subroutineName.name.
                    XmlAttribute subroutineInstanceBlockSubroutineNameName = xmlDocument.CreateAttribute("name");
                    subroutineInstanceBlockSubroutineNameName.Value = "SUBROUTINE_NAME";
                    subroutineInstanceBlockSubroutineName.Attributes?.Append(subroutineInstanceBlockSubroutineNameName);
                }
            }

            return subroutineInstanceBlock;
        }
        #endregion
    }
}