#ifndef CUDA_WRAPPER_CUH
    #define CUDA_WRAPPER_CUH
    
    #include "RGB.h"
    #include "cuda_runtime_api.h"
    
    /*
        RGB.h
        cuda_runtime_api.h
    */
    
    void Launch_DEVICE_GenerateFrame(const int blockCount,
                                     const int threadsPerBlock,
                                     const int xSize,
                                     const char *grayscaleCharacters,
                                     const int arraySize,
                                     const RGB *colors,
                                     char *frame);
    __global__ void DEVICE_GenerateFrame(const int xSize,
                                         const char *grayscaleCharacters,
                                         const int arraySize,
                                         const RGB *colors,
                                         char *frame);
#endif