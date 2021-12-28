using ConsoleVideo.Math;
using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleVideo.Media;

public sealed class FrameGenerator : ConsoleFrameGenerator {
    public FrameGenerator(Vector2Int windowSize, Vector2Int imageSize) {
        this.windowSize = windowSize;
        (widthScale, heightScale) = (((float)(imageSize.x) / windowSize.x), ((float)(imageSize.y) / windowSize.y));
        return;
    }

    public override IFrame Convert(Image<Bgr24> image) {
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
}