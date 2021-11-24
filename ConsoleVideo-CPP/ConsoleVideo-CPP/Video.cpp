#include <Video.hpp>
#include <iostream>

extern "C" {
    #include <libavformat/avformat.h>
    #include <libavcodec/avcodec.h>
	#include <libavutil/imgutils.h>
	#include <libswscale/swscale.h>
	#include <libswresample/swresample.h>
}

Video::Video(const std::string _filePath) {
	filePath = _filePath;

    AVFormatContext *pAvFormatContext = nullptr;

    if (avformat_open_input(&pAvFormatContext, _filePath.c_str(), nullptr, nullptr) != 0) {
        Dispose();
        throw std::exception("Failed to open input.");
    }

    if (avformat_find_stream_info(pAvFormatContext, nullptr) < 0) {
        Dispose();
        throw std::exception("Failed to find stream information.");
    }

    uint32_t videoStreamIndex = -1;
    for (uint32_t i = 0; i < pAvFormatContext->nb_streams; ++i) {
	    const AVCodecParameters *pAvCodecParameters = pAvFormatContext->streams[i]->codecpar;

	    if (pAvCodecParameters->codec_type == AVMEDIA_TYPE_VIDEO) {
            videoStreamIndex = i;

            AVCodec *pAvCodec = avcodec_find_decoder(pAvCodecParameters->codec_id);
	    }
    }

    AVCodecParameters *pAvCodecParameters = pAvFormatContext->streams[videoStreamIndex]->codecpar;
    AVCodec *pAvCodec = avcodec_find_decoder(pAvCodecParameters->codec_id);
    if (pAvCodec == nullptr) {
        Dispose();
        throw std::exception("Failed to open codec.");
    }

    AVCodecContext *pAvCodecContext = avcodec_alloc_context3(pAvCodec);
        /*, *pAvCodecContextOriginal;
    if (avcodec_copy_context(pAvCodecContext, pAvCodecContextOriginal) != 0) {
        Dispose();
        throw std::exception("Failed to copy codec context.");
    }
    */

    if (avcodec_open2(pAvCodecContext, pAvCodec, NULL) < 0) {
        Dispose();
        throw std::exception("Failed to open codec.");
    }

    AVFrame *pAvFrame = av_frame_alloc();
	return;
}

Video::~Video(void) {
    Dispose();
    return;
}

void Video::LoadNextFrame(void) {
    return;
}

void Video::Dispose(void) {
    std::cout << "Disposing." << std::endl;
    return;
}