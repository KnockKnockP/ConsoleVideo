using ConsoleVideo.Math;
using FFMediaToolkit.Decoding;
using System.Drawing;

namespace ConsoleVideo.Media; 

public sealed class Video {
    public readonly float ratio;
    public readonly double frameRate;
    public readonly Vector2Int resolution;
    public readonly MediaFile mediaFile;

    public Video(MediaFile _mediaFile) {
        //mediaFile
        mediaFile = _mediaFile ?? throw new MediaLoadException("Media file is null.");

        VideoStreamInfo videoStreamInfo = _mediaFile.Video.Info;

        //frames
        int? _frames = videoStreamInfo.NumberOfFrames;
        if (_frames == null) {
            throw new MediaLoadException("Number of frames is null.");
        }
        //frames = (int)(_frames);

        //frameRate
        frameRate = videoStreamInfo.AvgFrameRate;

        //resolution
        Size size = videoStreamInfo.FrameSize;
        resolution = new Vector2Int(size.Width, size.Height);

        //ratio
        ratio = ((float)(resolution.x) / resolution.y);
        return;
    }
}