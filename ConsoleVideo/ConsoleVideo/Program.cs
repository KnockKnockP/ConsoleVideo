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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml;

namespace ConsoleVideo {
    internal static class Program {
        private static readonly Vector2Int size = new(7, 7);
        private const bool inverted = false;

        [STAThread]
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

        [SuppressMessage("ReSharper.DPA",
                         "DPA0003: Excessive memory allocations in LOH",
                         MessageId = "type: System.Byte[]")]
        private static ExitCode Run() {
            InitializeFFmpeg(true);
            LoadVideo(out Video video);

            IList<IFrame<char>> frames = GenerateFrames(size,
                                                        video,
                                                        video.resolution);

            /*
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
                    1651 ~ 1684. Done.
                    1685 ~ 1697. Done.
                    1698 ~ 1718. Done.
                    1719 ~ 1739. Done.
                    1740 ~ 1779. Done.
                    1800 ~ 1822. Done.
                    1823 ~ 1842. Done.
                    1843 ~ 1870. Done.
                    1871 ~ 1887. Done.
                    1888 ~ 1901. Done.
                    1902 ~ 1913. Done.
                    1914 ~ 1954. Done.
                    1955 ~ 1995. Done.
                    
                    1996 ~ 2032. Done.
                    2033 ~ 2062. Done.
                    2063 ~ 2093. Done.
                    2094 ~ 2162. Done.
                    2163 ~ 2222. Done.
                    2223 ~ 2246. Done.
                    2247 ~ 2289. Done.
                    2290 ~ 2327. Done.
                    2328 ~ 2408. Done.
                    2409 ~ 2480. Done.
                    2481 ~ 2501. Done.
                    2502 ~ 2545. Done.
                    2546 ~ 2653. Done.
                    2654 ~ 2722. Done.
                    2723 ~ 2794. Done.
                    2795 ~ 2814. Done.
                    2815 ~ 2835. Done.
                    2836 ~ 2870. Done.
                    2871 ~ 2909. Done.
                    2910 ~ 2933. Done.
                    2934 ~ 2947. Done.
                    2948 ~ 3028. Done.
                    
                    3029 ~ 3116. Done.
                    3117 ~ 3175. Done.
                    3176 ~ 3239. Done.
                    3240 ~ 3273. Done.
                    3274 ~ 3307. Done.
                    3308 ~ 3376. Done.
                    3377 ~ 3397. Done.
                    3398 ~ 3412. Done.
                    3413 ~ 3423. Done.
                    3424 ~ 3440. Done.
                    3441 ~ 3457. Done.
                    3458 ~ 3467. Done.
                    3468 ~ 3475. Done.
                    3476 ~ 3492. Done.
                    3493 ~ 3505. Done.
                    3506 ~ 3513. Done.
                    3514 ~ 3522. Done.
                    3523 ~ 3534. Done.
                    3535 ~ 3551. Done.
                    3552 ~ 3571. Done.
                    3572 ~ 3582. Done.
                    3583 ~ 3598. Done.
                    3599 ~ 3616. Done.
                    3617 ~ 3625. Done.
                    3626 ~ 3647. Done.
                    3648 ~ 3656. Done.
                    3657 ~ 3669. Done.
                    3670 ~ 3677. Done.
                    3678 ~ 3705. Done.
                    3706 ~ 3748. Done.
                    3749 ~ 3766. Done.
                    3767 ~ 3804. Done.
                    3805 ~ 3819. Done.
                    3820 ~ 3845. Done.
                    3846 ~ 3883. Done.
                    3884 ~ 3917. Done.
                    3918 ~ 3941. Done.
                    3942 ~ 3960. Done.
                    3961 ~ 3980. Done.
                    3981 ~ 4011. Done.
                    
                    4012 ~ 4052. Done.
                    4053 ~ 4088. Done.
                    4089 ~ 4142. Done.
                    4143 ~ 4158. Done.
                    4159 ~ 4184. Done.
                    4185 ~ 4226. Done.
                    4227 ~ 4246. Done.
                    4247 ~ 4262. Done.
                    4263 ~ 4311. Done.
                    4312 ~ 4362. Done.
                    4363 ~ 4388. Done.
                    4389 ~ 4400. Done.
                    4401 ~ 4420. Done.
                    4421 ~ 4492. Done.
                    4493 ~ 4544. Done.
                    4545 ~ 4569. Done.
                    4570 ~ 4595. Done.
                    4596 ~ 4623. Done.
                    4624 ~ 4665. Done.
                    4666 ~ 4675. Done.
                    4676 ~ 4690. Done.
                    4691 ~ 4728. Done.
                    4729 ~ 4766. Done.
                    4767 ~ 4792. Done.
                    4793 ~ 4841. Done.
                    4842 ~ 4863. Done.
                    4864 ~ 4887. Done.
                    4888 ~ 4934. Done.
                    4935 ~ 4970. Done.
                    4971 ~ 5009. Done.
                    
                    5010 ~ 5028. Done.
                    5029 ~ 5055. Done.
                    5056 ~ 5119. Done.
                    5120 ~ 5162. Done.
                    5163 ~ 5186. Done.
                    5187 ~ 5209. Done.
                    5210 ~ 5229. Done.
                    5230 ~ 5268. Done.
                    5269 ~ 5287. Done.
                    5288 ~ 5304. Done.
                    5305 ~ 5320. Done.
                    5321 ~ 5336. Done.
                    5337 ~ 5344. Done.
                    5345 ~ 5355. Done.
                    5356 ~ 5369. Done.
                    5370 ~ 5398. Done.
                    5399 ~ 5410. Done.
                    5411 ~ 5430. Done.
                    5431 ~ 5488. Done.
                    5489 ~ 5516. Done.
                    5517 ~ 5535. Done.
                    5536 ~ 5554. Done.
                    5555 ~ 5577. Done.
                    5578 ~ 5608. Done.
                    5609 ~ 5629. Done.
                    5630 ~ 5660. Done.
                    5561 ~ 5683. Done.
                    5684 ~ 5706. Done.
                    5707 ~ 5739. Done.
                    5740 ~ 5776. Done.
                    5777 ~ 5827. Done.
                    5828 ~ 5858. Done.
                    5859 ~ 5944. Done.
                    5945 ~ 5961. Done.
                    5962 ~ 6044. Done.
                    
                    6045 ~ 6103. Done.
                    6104 ~ 6129. Done.
                    6130 ~ 6152. Done.
                    6153 ~ 6168. Done.
                    6169 ~ 6196. Done.
                    6197 ~ 6257. Done.
                    6258 ~ 6278. Done.
                    6279 ~ 6297. Done.
                    6298 ~ 6311. Done.
                    6312 ~ 6322. Done.
                    6323 ~ 6332. Done.
                    6333 ~ 6342. Done.
                    6343 ~ 6350. Done.
                    6351 ~ 6360. Done.
                    6361 ~ 6370. Done.
                    6371 ~ 6380. Done.
                    6381 ~ 6398. Done.
                    6399 ~ 6430. Done.
                    6431 ~ 6457. Done.
                    6458 ~ 6471. Done.
                    6472 ~ 6484. Done.
                    6485 ~ 6514. Done.
                    6515 ~ 6560. Done.
                    
                    The end.
            */
            const int startFrameInclusive = 6561;
            CharFrame baseFrame = (CharFrame)(frames[(frames.Count - 1)]);

            const string filePath = @"C:\Users\memeb\Desktop\xml.xml";
            int endFrameInclusive = GenerateXml(frames,
                                                filePath,
                                                baseFrame,
                                                startFrameInclusive,
                                                10000f);

            Thread.Sleep(500);
            string videoName = $"{startFrameInclusive} to {endFrameInclusive}.mp4";
            Clipboard.SetText(videoName);

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

            frames.Add(new CharFrame(windowSize));
            for (int i = 0; i < (windowSize.x * windowSize.y); ++i) {
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
                                       int startFrameInclusive,
                                       float maxFileSize) {
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

                            using MemoryStream memoryStream = new();
                            using XmlWriter xmlWriterMemory = XmlWriter.Create(memoryStream, xmlWriterSettings);
                            xmlDocument.WriteTo(xmlWriterMemory);
                            
                            returnI = i;
                            if ((memoryStream.Length * 0.001) > maxFileSize) {
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

            using StreamReader streamReader = new(File.Open(filePath, FileMode.Open));
            Clipboard.SetText(streamReader.ReadToEnd());
            streamReader.Close();
            streamReader.Dispose();

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
                    VariableReferenceBlock variableReferenceBlock = new(xmlDocument,
                                                                        variable,
                                                                        objectInstance);
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