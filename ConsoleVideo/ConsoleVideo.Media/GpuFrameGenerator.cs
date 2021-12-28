using ConsoleVideo.Math;
using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.OpenCL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;

namespace ConsoleVideo.Media;

public sealed class GpuFrameGenerator : ConsoleFrameGenerator, IDisposable {
    private readonly Context context;
    private readonly Accelerator accelerator;

    private readonly Index1D windowSizeXIndex;
    private readonly Bgr[] rowConverted;
    private readonly MemoryBuffer1D<Bgr, Stride1D.Dense> deviceRow;
    private readonly MemoryBuffer1D<sbyte, Stride1D.Dense> deviceOutput;
    private readonly MemoryBuffer1D<sbyte, Stride1D.Dense> deviceCharacters;

    private readonly sbyte[] output;

    public GpuFrameGenerator(Vector2Int windowSize, Vector2Int imageSize) {
        this.windowSize = windowSize;
        (widthScale, heightScale) = (((float)(imageSize.x) / windowSize.x), ((float)(imageSize.y) / windowSize.y));

        context = Context.CreateDefault();
        Device device = context.Devices[0];
        accelerator = device.CreateAccelerator(context);


        if ((accelerator is CPUAccelerator) && (context.Devices.Length > 1)) {
            device = context.Devices[1];
        
            accelerator.Dispose();
            accelerator = device.CreateAccelerator(context);
        }

        windowSizeXIndex = new Index1D(windowSize.x);
        rowConverted = new Bgr[imageSize.x];
        deviceRow = accelerator.Allocate1D<Bgr>(imageSize.x);
        deviceOutput = accelerator.Allocate1D<sbyte>(windowSize.x);
        deviceCharacters = accelerator.Allocate1D(ToSByteArray(grayscaleCharacters));
        
        output = new sbyte[windowSize.x];

        Console.Write($"Using: {accelerator}.\r\n");
        return;
    }

    ~GpuFrameGenerator() => Dispose();

    public void Dispose() {
        if (!context.IsDisposed) {
            context.Dispose();
        }
        if (!accelerator.IsDisposed) {
            accelerator.Dispose();
        }
        if (!deviceCharacters.IsDisposed) {
            deviceCharacters.Dispose();
        }
        return;
    }

    public override IFrame Convert(Image<Bgr24> image) {
        IFrame frame = new SByteFrame(windowSize);

        for (int y = 0; y < windowSize.y; ++y) {
            Span<Bgr24> row = image.GetPixelRowSpan((int)(y * heightScale));
            for (int i = 0; i < row.Length; ++i) {
                Bgr24 pixel = row[i];
                Bgr convertedPixel = new(pixel.R, pixel.G, pixel.B);

                rowConverted[i] = convertedPixel;
            }
            deviceRow.CopyFromCPU(rowConverted);

            var kernel = accelerator.LoadAutoGroupedKernel<Index1D, ArrayView1D<sbyte, Stride1D.Dense>, ArrayView1D<Bgr, Stride1D.Dense>, ArrayView1D<sbyte, Stride1D.Dense>, float>(ConvertKernel);
            kernel(accelerator.DefaultStream,
                   windowSizeXIndex,
                   deviceCharacters,
                   deviceRow,
                   deviceOutput,
                   widthScale);
            accelerator.Synchronize();
            
            deviceOutput.CopyToCPU(output);

            for (int x = 0; x < windowSize.x; ++x) {
                //I don't like this...
                frame.SetPixel(y, x, output[x]);
            }
        }
        return frame;
    }

    private static void ConvertKernel(Index1D index1D, ArrayView1D<sbyte, Stride1D.Dense> characters, ArrayView1D<Bgr, Stride1D.Dense> row, ArrayView1D<sbyte, Stride1D.Dense> output, float widthScale) {
        Bgr color = row[(int)(index1D * widthScale)];
        
        double average = ((color.R + color.G + color.B) / 3d);

        double index = (average / 255d);
        index *= 10d;
        index = System.Math.Round(index);

        if (index < 0d) {
            index = 0;
        } else if (index >= 10d) {
            index = 9d;
        }

        output[index1D] = characters[(int)(index)];
        return;
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