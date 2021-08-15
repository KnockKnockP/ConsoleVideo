#ifndef CUDA_HPP
#define CUDA_HPP

using namespace ConsoleVideo::Math;
using namespace SixLabors::ImageSharp::PixelFormats;
using namespace SixLabors::ImageSharp;

/*
    ConsoleVideo::Math
    SixLabors::ImageSharp::PixelFormats
    SixLabors::ImageSharp
*/

class CUDAWrapperNative {
public:
    int32_t frameSize = 0,
            windowSizeX = 0,
            windowSizeY = 0,
            grayscaleCharactersLength = 0;
    char *deviceGrayscaleCharacters = nullptr;

    float widthScale,
          heightScale;

    struct RGB *colors = nullptr;

    bool colorConvertSuccess = true;
};

namespace ConsoleVideo::CUDA {
    public ref class CUDAWrapper sealed {
        CUDAWrapperNative *cudaWrapperNative;
        Image<Rgb24> ^image;

    public:
        bool SetupCUDA(const array<char> ^grayscaleCharacters,
                       const Vector2Int windowSize,
                       const float widthScale,
                       const float heightScale);

        array<char>^ ConvertFrame(Image<Rgb24> ^image);

        void SetupColors(int y);

        ~CUDAWrapper();
    };
}
#endif