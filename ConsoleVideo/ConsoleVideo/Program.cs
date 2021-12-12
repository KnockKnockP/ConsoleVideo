﻿using ConsoleVideo.IO;
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml;

namespace ConsoleVideo {
    internal static class Program {
        private static readonly Vector2Int size = new(7, 7);
        private const bool inverted = false;

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

        [SuppressMessage("ReSharper.DPA", "DPA0003: Excessive memory allocations in LOH")]
        private static ExitCode Run() {
            InitializeFFmpeg(true);
            LoadVideo(out Video video);

            IList<IFrame<char>> frames = GenerateFrames(size,
                                                        video,
                                                        video.resolution);

            /*
                We have about 40kb (700 blocks) of space to work with.
                
                Total frame count: 6560 frames.
                
                List of frames that I have recorded:
                    0 ~ 75. Done.
                    76 ~ 130. Done.
                    131 ~ 180. Done.
                    181 ~ 220. Done.
                    221 ~ 300. Done.
                    301 ~ 340. Done.
                    341 ~ 360. Done.
                    361 ~ 400. Done.
                    401 ~ 440. Done.
                    441 ~ 450. Done.
                    451 ~ 465. Done.
                    466 ~ 480. Done.
                    481 ~ 520. Done.
                    521 ~ 540. Done.
                    541 ~ 580. Done.
                    581 ~ 640. Done.
                    641 ~ 655. Done.
                    656 ~ 700. Done.
                    701 ~ 760. Done.
                    761 ~ 810. Done.
                    811 ~ 870. Done.
                    871 ~ 900. Done.
                    901 ~ 950. Done.
                    951 ~ 1000. Done.
                    
                    1001 ~ 1050. Done.
                    1051 ~ 1070. Done.
                    1071 ~ 1090. Done.
                    1091 ~ 1110. Done.
                    1111 ~ 1150. Done.
                    1151 ~ 1190. Done.
                    1191 ~ 1223. Done.
                    1224 ~ 1250. Done.
                    1251 ~ 1275. Done.
                    1276 ~ 1330. Done.
                    1331 ~ 1365. Done.
                    1366 ~ 1430. Done.
                    1431 ~ 1475. Done.
                    1476 ~ 1510. Done.
                    1511 ~ 1517. Done.
                    1518 ~ 1530. Done.
                    1531 ~ 1560. Done.
                    1561 ~ 1585. Done.
                    1586 ~ 1620. Done.
                    1621 ~ 1650. Done.
                    1651 ~ 1684.
            */
            const int startFrameInclusive = 1651;
            CharFrame baseFrame = (CharFrame)(frames[(frames.Count - 1)]);

            const string filePath = @"C:\Users\memeb\Desktop\xml.xml";
            int endFrameInclusive = GenerateXml(frames,
                        filePath,
                        baseFrame,
                        startFrameInclusive);
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

        private static void LoadVideo(out Video video) => video = new Video(MediaFile.Open(@"D:\GitHub\ConsoleVideo\ConsoleVideo\ConsoleVideo\Resources\Videos\Original.mp4"));

        private static IList<IFrame<char>> GenerateFrames(Vector2Int windowSize,
                                                          Video video,
                                                          Vector2Int videoSize) {
            FrameGenerator frameGenerator = new(windowSize,
                                                video.resolution.x,
                                                video.resolution.y,
                                                inverted);

            IList<IFrame<char>> frames = new List<IFrame<char>>();

            int totalFrameCount = 0;
            while (video.mediaFile.Video.TryGetNextFrame(out ImageData imageData) == true) {
                using (Image<Rgb24> image = Image.LoadPixelData<Rgb24>(imageData.Data,
                                                                       videoSize.x,
                                                                       videoSize.y)) {
                    frames.Add(frameGenerator.Convert(image));
                    image.Dispose();
                }
                Console.Write($"\r{++totalFrameCount} frames converted to convertable format.");
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
            
            for (int i = startFrameInclusive; i <= endFrameInclusive; ++i) {
                IFrame<char> frame = frames[i];

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
            }
            return;
        }

        #region Xml Generation.
        private static int GenerateXml(IList<IFrame<char>> frames,
                                        string filePath,
                                        CharFrame baseFrame,
                                        int startFrameInclusive) {
            XmlWriterSettings xmlWriterSettings = new() {
                Async = false,
                CheckCharacters = true,
                CloseOutput = true,
                ConformanceLevel = ConformanceLevel.Auto,
                DoNotEscapeUriAttributes = true,
                Encoding = Encoding.Default,
                Indent = false,
                IndentChars = "",
                NamespaceHandling = NamespaceHandling.OmitDuplicates,
                NewLineChars = "\n"
            };
            
            XmlDocument xmlDocument = new();

            int returnI = 0;

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


                    using MemoryStream memoryStream = new();
                    using XmlWriter xmlWriterMemory = XmlWriter.Create(memoryStream, xmlWriterSettings);
                    {
                        XmlNode previousBlock = null;

                        for (int i = startFrameInclusive; i < frames.Count; ++i) {
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

                                    XmlNode spotBlock = ((activated == true) ? GenerateSpotTargetBlock(xmlDocument, j) : GenerateUnspotTargetBlock(xmlDocument, j));

                                    if (previousBlock == null) {
                                        actionsBlock.AppendChild(spotBlock);
                                    } else {
                                        XmlNode nextBlock = GenerateNextBlock(xmlDocument);
                                        previousBlock.AppendChild(nextBlock);
                                        nextBlock.AppendChild(spotBlock);
                                    }
                                    previousBlock = spotBlock;
                                }
                            }

                            XmlNode sleepSubroutineBlock = GenerateSubroutineInstanceBlock(xmlDocument, "Sleep"),
                                    nextBlockForSleep = GenerateNextBlock(xmlDocument);
                            previousBlock?.AppendChild(nextBlockForSleep);
                            nextBlockForSleep.AppendChild(sleepSubroutineBlock);

                            previousBlock = sleepSubroutineBlock;

                            memoryStream.SetLength(0);
                            xmlDocument.WriteTo(xmlWriterMemory);
                            if ((memoryStream.Length * 0.001) > 50) {
                                returnI = i;
                                break;
                            }
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
            
            using XmlWriter xmlWriterFile = XmlWriter.Create(filePath, xmlWriterSettings);
            xmlDocument.Save(xmlWriterFile);

            xmlWriterFile.Close();
            xmlWriterFile.Dispose();

            FileInfo fileInfo = new(filePath);
            Console.Write($"File size: {(fileInfo.Length * 0.001)} kilobytes.\r\n");
            return returnI;
        }

        private static XmlNode GenerateSpotTargetBlock(XmlDocument xmlDocument, int botsIndex) {
            //Spot target block.
            XmlNode spotTargetBlock = xmlDocument.CreateElement("block");

            {
                //Spot target.type
                XmlAttribute spotTargetType = xmlDocument.CreateAttribute("type");
                spotTargetType.Value = "SpotTarget";
                spotTargetBlock.Attributes?.Append(spotTargetType);
            }

            {
                //Spot target's first parameter.
                XmlNode spotTargetBlockFirstParameter = xmlDocument.CreateElement("value");
                spotTargetBlock.AppendChild(spotTargetBlockFirstParameter);

                {
                    //Spot target's first parameter.name.
                    XmlAttribute spotTargetBlockFirstParameterName = xmlDocument.CreateAttribute("name");
                    spotTargetBlockFirstParameterName.Value = "VALUE-0";
                    spotTargetBlockFirstParameter.Attributes?.Append(spotTargetBlockFirstParameterName);
                }

                {
                    XmlNode parameterContent = GenerateValueInArray(xmlDocument,
                                                                    Variables.Global.bots,
                                                                    null,
                                                                    botsIndex);
                    spotTargetBlockFirstParameter.AppendChild(parameterContent);
                }
            }

            {
                //Spot target's second parameter.
                XmlNode spotTargetBlockSecondParameter = xmlDocument.CreateElement("value");
                spotTargetBlock.AppendChild(spotTargetBlockSecondParameter);

                {
                    //Spot target's second parameter.name.
                    XmlAttribute spotTargetBlockSecondParameterName = xmlDocument.CreateAttribute("name");
                    spotTargetBlockSecondParameterName.Value = "VALUE-1";
                    spotTargetBlockSecondParameter.Attributes?.Append(spotTargetBlockSecondParameterName);
                }

                {
                    XmlNode parameterContent = GenerateNumberLiteral(xmlDocument, 5000);
                    spotTargetBlockSecondParameter.AppendChild(parameterContent);
                }
            }

            return spotTargetBlock;
        }

        private static XmlNode GenerateUnspotTargetBlock(XmlDocument xmlDocument, int botsIndex) {
            //Unspot target block.
            XmlNode unspotTargetBlock = xmlDocument.CreateElement("block");

            {
                //Unspot target.type
                XmlAttribute spotTargetType = xmlDocument.CreateAttribute("type");
                spotTargetType.Value = "UnspotTarget";
                unspotTargetBlock.Attributes?.Append(spotTargetType);
            }

            {
                //Unspot target's first parameter.
                XmlNode unspotTargetBlockFirstParameter = xmlDocument.CreateElement("value");
                unspotTargetBlock.AppendChild(unspotTargetBlockFirstParameter);

                {
                    //Unspot target's first parameter.name.
                    XmlAttribute unspotTargetBlocksFirstParameterName = xmlDocument.CreateAttribute("name");
                    unspotTargetBlocksFirstParameterName.Value = "VALUE-0";
                    unspotTargetBlockFirstParameter.Attributes?.Append(unspotTargetBlocksFirstParameterName);
                }

                {
                    XmlNode parameterContent = GenerateValueInArray(xmlDocument,
                                                                    Variables.Global.bots,
                                                                    null,
                                                                    botsIndex);
                    unspotTargetBlockFirstParameter.AppendChild(parameterContent);
                }
            }

            return unspotTargetBlock;
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