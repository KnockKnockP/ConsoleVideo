using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ConsoleVideo.Media {
    public interface IFrameGenerator {
        public IFrame Convert(Image<Bgr24> image);
    }
}