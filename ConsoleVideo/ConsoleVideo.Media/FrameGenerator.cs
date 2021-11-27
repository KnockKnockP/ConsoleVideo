using ConsoleVideo.Math;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Threading.Tasks;

namespace ConsoleVideo.Media {
    public class FrameGenerator {
        private readonly Vector2Int windowSize;

        private readonly float widthScale,
                               heightScale;

        public FrameGenerator(Vector2Int _windowSize,
                              int _imageWidth,
                              int _imageHeight) {
            windowSize = _windowSize;
            (widthScale, heightScale) = (((float)(_imageWidth) / windowSize.x), ((float)(_imageHeight) / windowSize.y));
            return;
        }

        public IFrame<char> Convert(Image<Rgb24> image) {
            IFrame<char> frame = new CharFrame(windowSize);
            Parallel.For(0,
                         windowSize.y,
                         (int y, ParallelLoopState _) => {
                             int yArray = (int)(System.Math.Round(y * heightScale));

                             for (int x = 0; x < windowSize.x; ++x) {
                                 int xArray = (int)(System.Math.Round(x * widthScale));

                                 Rgb24 color = image[xArray, yArray];
                                 byte average = (byte)((color.R + color.G + color.B) / 3);

                                 char pixel = '#';
                                 if (average == 0) {
                                     pixel = ' ';
                                 }

                                 frame.SetPixel(y,
                                                x,
                                                pixel);
                             }
                         });

            return frame;
        }
    }
}