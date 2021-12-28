using ConsoleVideo.Math;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleVideo.Media {
    public class FrameGenerator : IFrameGenerator{
        private readonly Vector2Int windowSize;

        private readonly float widthScale,
                               heightScale;

        private static readonly char[] grayscaleCharacters = {
            ' ',
            '.',
            '|',
            '+',
            'S',
            '$',
            'L',
            '&',
            '#',
            '@'
        };

        private static readonly int arraySize = grayscaleCharacters.Length;

        public FrameGenerator(Vector2Int windowSize,
                              int imageWidth,
                              int imageHeight) {
            this.windowSize = windowSize;
            (widthScale, heightScale) = (((float)(imageWidth) / windowSize.x), ((float)(imageHeight) / windowSize.y));
            return;
        }

        public IFrame Convert(Image<Bgr24> image) {
            IFrame frame = new CharFrame(windowSize);
            
            Parallel.For(0,
                         windowSize.y,
                         (int y, ParallelLoopState _) => {
                             int yArray = (int)(System.Math.Round(y * heightScale));

                             for (int x = 0; x < windowSize.x; ++x) {
                                 int xArray = (int)(System.Math.Round(x * widthScale));

                                 Bgr24 color = image[xArray, yArray];
                                 byte average = (byte)((color.R + color.G + color.B) / 3);

                                 float index = ((float)(average) / byte.MaxValue);
                                 index *= arraySize;
                                 index = (float)(System.Math.Round(index));

                                 if (index < 0f) {
                                     index = 0;
                                 } else if (index >= arraySize) {
                                     index = (arraySize - 1);
                                 }

                                 frame.SetPixel(y,
                                                x,
                                                grayscaleCharacters[(int)(index)]);
                             }
                         });
            return frame;
        }

        private static sbyte[] ToSByteArray(IReadOnlyList<char> array) {
            int length = array.Count;

            sbyte[] result = new sbyte[length];
            for (int i = 0; i < length; ++i) {
                result[i] = (sbyte)(array[i]);
            }
            return result;
        }
    }
}