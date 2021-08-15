#include "CUDAWrapper.cuh"
#include "CUDAWrapper.hpp"
#include "RGB.h"
#include <cstdlib>
#include "cuda_runtime_api.h"
#include <stdio.h>

/*
    CUDAWrapper.cuh
    CUDAWrapper.hpp
    RGB.h
    cstdlib
    cuda_runtime_api.h
    stdio.h
*/

using namespace ConsoleVideo::Math;
using namespace PixelFormats;
using namespace SixLabors::ImageSharp;
using namespace System;
using namespace Threading::Tasks;

/*
    ConsoleVideo::Math
    PixelFormats
    SixLabors::ImageSharp
    System
    Threading::Tasks
*/

namespace ConsoleVideo::CUDA {
    bool CUDAWrapper::SetupCUDA(const array<char> ^grayscaleCharacters,
                                const Vector2Int windowSize,
                                const float widthScale,
                                const float heightScale) {
        bool success = true;

        cudaWrapperNative = new CUDAWrapperNative;
        cudaWrapperNative->frameSize = (windowSize.x * windowSize.y);
        cudaWrapperNative->windowSizeX = windowSize.x;
        cudaWrapperNative->windowSizeY = windowSize.y;
        cudaWrapperNative->grayscaleCharactersLength = grayscaleCharacters->Length;
        cudaWrapperNative->widthScale = widthScale;
        cudaWrapperNative->heightScale = heightScale;

        cudaError_t cudaStatus = cudaMalloc(reinterpret_cast<void**>(&cudaWrapperNative->deviceGrayscaleCharacters),
                                            (cudaWrapperNative->grayscaleCharactersLength * sizeof(char)));
        if (cudaStatus != cudaSuccess) {
            fprintf(stdout, "cudaMalloc cudaWrapperNative->nativeGrayscaleCharacters failed.\r\n");
            success = false;
            goto ERROR_GOTO;
        }

        for (int i = 0; i < cudaWrapperNative->grayscaleCharactersLength; ++i) {
            char temp = grayscaleCharacters[i];
            cudaStatus = cudaMemcpy(&cudaWrapperNative->deviceGrayscaleCharacters[i],
                                    &temp,
                                    sizeof(char),
                                    cudaMemcpyHostToDevice);
            if (cudaStatus != cudaSuccess) {
                fprintf(stdout, "cudaMemcpy nativeGrayscaleCharacters, temp failed (i: %d).\r\n.", i);
                success = false;
                goto ERROR_GOTO;
            }
        }

    ERROR_GOTO:
        if (success == false) {
            cudaFree(cudaWrapperNative->deviceGrayscaleCharacters);
        }
        return success;
    }

    /*
        Hot flaming shit.
        Moral of the story, never, ever try to use CUDA with .NET.
    */
    array<char>^ CUDAWrapper::ConvertFrame(Image<Rgb24> ^_image) {
        char *frame = nullptr;
        char *hostFrame = nullptr;

        image = _image;

        cudaError_t cudaStatus = cudaMalloc(reinterpret_cast<void**>(&cudaWrapperNative->colors), (cudaWrapperNative->frameSize * sizeof(RGB)));
        if (cudaStatus != cudaSuccess) {
            fprintf(stdout, "cudaMalloc colors failed.\r\n");
            goto ERROR_GOTO;
        }

        Action<int> ^action = gcnew Action<int>(this, &CUDAWrapper::SetupColors);
        Parallel::For(0, cudaWrapperNative->windowSizeY, action);
        if (cudaWrapperNative->colorConvertSuccess == false) {
            fprintf(stdout, "Color conversion failed.\r\n");
            goto ERROR_GOTO;
        }

        cudaStatus = cudaMalloc(reinterpret_cast<void**>(&frame), (cudaWrapperNative->frameSize * sizeof(char)));
        if (cudaStatus != cudaSuccess) {
            fprintf(stdout, "cudaMalloc frame failed.\r\n");
            goto ERROR_GOTO;
        }
        
        Launch_DEVICE_GenerateFrame(cudaWrapperNative->windowSizeY,
                                    cudaWrapperNative->windowSizeX,
                                    cudaWrapperNative->windowSizeX,
                                    cudaWrapperNative->deviceGrayscaleCharacters,
                                    cudaWrapperNative->grayscaleCharactersLength,
                                    cudaWrapperNative->colors,
                                    frame);

        cudaStatus = cudaDeviceSynchronize();
        if (cudaStatus != cudaSuccess) {
            fprintf(stdout, "cudaDeviceSynchronize returned %d.\r\n", cudaStatus);
            goto ERROR_GOTO;
        }
        
        hostFrame = static_cast<char*>(malloc(cudaWrapperNative->frameSize * sizeof(char)));
        cudaStatus = cudaMemcpy(hostFrame,
                                frame,
                                (cudaWrapperNative->frameSize * sizeof(char)),
                                cudaMemcpyDeviceToHost);
        if (cudaStatus != cudaSuccess) {
            fprintf(stdout, "cudaMemcpy frame to hostFrame failed.\r\n");
            goto ERROR_GOTO;
        }

        array<char> ^returnArray = gcnew array<char>(cudaWrapperNative->frameSize);
        for (int i = 0; i < returnArray->Length; ++i) {
            returnArray[i] = hostFrame[i];
        }

    ERROR_GOTO:
        cudaFree(cudaWrapperNative->colors);
        free(hostFrame);
        cudaFree(frame);
        return returnArray;
    }

    void CUDAWrapper::SetupColors(int y) {
        const int yArray = roundf(y * cudaWrapperNative->heightScale);

        for (int x = 0; x < cudaWrapperNative->windowSizeX; ++x) {
            const int xArray = roundf(x * cudaWrapperNative->widthScale);

            Rgb24 rgb24 = image[xArray, yArray];
            RGB rgb;
            rgb.r = rgb24.R;
            rgb.g = rgb24.G;
            rgb.b = rgb24.B;

            if (cudaMemcpy(&cudaWrapperNative->colors[ArrayMath::GetIndex(y, x, cudaWrapperNative->windowSizeX)],
                           &rgb,
                           sizeof(RGB),
                           cudaMemcpyHostToDevice) != cudaSuccess) {
                cudaWrapperNative->colorConvertSuccess = false;
                return;
            }
        }
        return;
    }

    CUDAWrapper::~CUDAWrapper() {
        cudaFree(cudaWrapperNative->deviceGrayscaleCharacters);
        delete cudaWrapperNative;
        return;
    }

}