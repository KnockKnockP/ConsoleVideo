using System;

namespace ConsoleVideo.IO; 

public static class FilePath {
    public const string FfmpegFolder = "ffmpeg";

    public static string ApplicationDirectory => AppDomain.CurrentDomain.BaseDirectory;
}