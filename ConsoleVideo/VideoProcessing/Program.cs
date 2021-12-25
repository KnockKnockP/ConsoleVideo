using ConsoleVideo.IO;
using ConsoleVideo.Media;
using FFMediaToolkit;
using FFMediaToolkit.Decoding;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;
using Microsoft.WindowsAPICodePack.Dialogs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Security;

LoadFFmpeg();
Video video = LoadVideo();

int frame = 0;
ImageData startingImageData = new();

Bgr24 normalColor = new(0,
                        0,
                        0);
while (video.mediaFile.Video.TryGetNextFrame(out ImageData imageData)) {
    using Image<Bgr24> image = Image.LoadPixelData<Bgr24>(imageData.Data,
                                                          video.resolution.x,
                                                          video.resolution.y);

    Bgr24 pixel = image[1083, 869];
    normalColor.R += pixel.R;
    normalColor.G += pixel.G;
    normalColor.B += pixel.B;

    int divide = (frame + 1);
    normalColor.R = (byte)(normalColor.R / divide);
    normalColor.G = (byte)(normalColor.G / divide);
    normalColor.B = (byte)(normalColor.B / divide);

    if (pixel.R > (normalColor.R + 80)) {
        Console.Write($"Found a suspiciously red pixel at frame {frame}.\r\n");

        startingImageData = imageData;
        break;
    }
    
    Console.Write($"Checked frame {frame} for the start.\r\n");
    ++frame;
}

VideoEncoderSettings videoEncoderSettings = new(video.resolution.x,
                                                video.resolution.y,
                                                60,
                                                VideoCodec.H264) {
    EncoderPreset = EncoderPreset.VerySlow,
    Bitrate = (int)(video.mediaFile.Info.Bitrate),
    CRF = 40
};
using MediaOutput mediaOutput = MediaBuilder.CreateContainer($@"C:\Users\memeb\Desktop\{video.mediaFile.Info.FilePath.Split('\\').Last()}")
                                            .WithVideo(videoEncoderSettings)
                                            .Create();

mediaOutput.Video.AddFrame(startingImageData);
while (video.mediaFile.Video.TryGetNextFrame(out ImageData imageData)) {
    using Image<Bgr24> image = Image.LoadPixelData<Bgr24>(imageData.Data,
                                                          video.resolution.x,
                                                          video.resolution.y);

    Bgr24 pixel = image[1083, 869];

    const int threshold = 20;
    if ((pixel.R <= (normalColor.R + threshold)) && (pixel.R >= (normalColor.R - threshold))) {
        Console.Write($"Found a suspiciously not so red pixel at frame {frame}.\r\n");
        break;
    }
    
    Console.Write($"Checked frame {frame} for the end.\r\n");
    ++frame;
    
    mediaOutput.Video.AddFrame(imageData);
}
mediaOutput.Dispose();
return;

void LoadFFmpeg() {
    FFmpegLoader.FFmpegPath = IoUtilities.FindDirectory("ffmpeg");
    FFmpegLoader.LoadFFmpeg();
    return;
}

Video LoadVideo() {
    using CommonOpenFileDialog commonOpenFileDialog = new("Select a video to play.");
    commonOpenFileDialog.Filters.Add(new CommonFileDialogFilter("mp4 video", "*.mp4"));
    commonOpenFileDialog.Filters.Add(new CommonFileDialogFilter("AVI video", "*.avi"));

    commonOpenFileDialog.ShowDialog();

    return new Video(MediaFile.Open(commonOpenFileDialog.FileName));
}