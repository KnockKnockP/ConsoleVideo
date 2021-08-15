#include "CUDAWrapper.cuh"
#include "RGB.h"
#include <cstdint>

/*
    CUDAWrapper.cuh
    RGB.h
    cstdint
*/

void Launch_DEVICE_GenerateFrame(const int blockCount,
                                 const int threadsPerBlock,
                                 const int xSize,
                                 const char *grayscaleCharacters,
                                 const int arraySize,
                                 const RGB *colors,
                                 char *frame) {
    DEVICE_GenerateFrame<<<blockCount, threadsPerBlock>>>(xSize, grayscaleCharacters, arraySize, colors, frame);
    return;
}
                                 
__global__ void DEVICE_GenerateFrame(const int xSize,
                                     const char *grayscaleCharacters,
                                     const int arraySize,
                                     const RGB *colors,
                                     char *frame) {
    int x = threadIdx.x,
        y = blockIdx.x;
    
    RGB color = colors[((xSize * y) + x)];
    int16_t average = ((color.r + color.g + color.b) / 3);
    
    float index = ((float)(average) / 255);
    index *= arraySize;
    index = roundf(index);
    
    if (index < 0) {
        index = 0;
    } else if (index >= arraySize) {
        index = (arraySize - 1);
    }
    
    frame[((xSize * y) + x)] = grayscaleCharacters[(int)(index)];
    return;
}