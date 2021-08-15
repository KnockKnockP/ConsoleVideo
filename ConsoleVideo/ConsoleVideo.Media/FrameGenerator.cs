using ConsoleVideo.CUDA;
using ConsoleVideo.Math;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleVideo.Media {
    public class FrameGenerator {
        private readonly bool useCUDA;
        private CUDAWrapper cudaWrapper;

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

        public FrameGenerator(bool _useCUDA,
                              Vector2Int _windowSize,
                              int _imageWidth,
                              int _imageHeight) {
            (useCUDA, windowSize) = (_useCUDA, _windowSize);
            (widthScale, heightScale) = (((float)(_imageWidth) / windowSize.x), ((float)(_imageHeight) / windowSize.y));

            if (useCUDA == true) {
                SetupCUDA();
            }
            return;
        }

        ~FrameGenerator() => cudaWrapper?.Dispose();

        private void SetupCUDA() {
            cudaWrapper = new CUDAWrapper();
            cudaWrapper.SetupCUDA(ToSByteArray(grayscaleCharacters),
                                  windowSize,
                                  widthScale,
                                  heightScale);
            return;
        }

        public IFrame Convert(Image<Rgb24> image) {
            IFrame frame;

            if (useCUDA == true) {
                frame = new SByteFrame(windowSize, cudaWrapper.ConvertFrame(image));
            } else {
                frame = new CharFrame(windowSize);
                Parallel.For(0,
                             windowSize.y,
                             (int y, ParallelLoopState _) => {
                                 int yArray = (int)(System.Math.Round(y * heightScale));

                                 for (int x = 0; x < windowSize.x; ++x) {
                                     int xArray = (int)(System.Math.Round(x * widthScale));

                                     Rgb24 color = image[xArray, yArray];
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
            }
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