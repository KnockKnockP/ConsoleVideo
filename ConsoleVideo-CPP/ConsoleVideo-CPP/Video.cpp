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
	return;
}

Video::~Video(void) {
    Dispose();
    return;
}

void Video::LoadCodec(void) {
    avFormatContext = avformat_alloc_context();
    if (avFormatContext == nullptr) {
        throw std::exception("Failed to allocate AVFormatContext.");
    }

    if (avformat_open_input(&avFormatContext, filePath.c_str(), nullptr, nullptr) != 0) {
        const std::string message("Failed to open file at path " + filePath);
        throw std::exception(message.c_str());
    }
    if (avformat_find_stream_info(avFormatContext, nullptr) < 0) {
        throw std::exception("Failed to find stream information.");
    }

    streamPacks = static_cast<StreamPack*>(malloc(sizeof(StreamPack) * avFormatContext->nb_streams));

    for (uint32_t i = 0; i < avFormatContext->nb_streams; ++i) {
    	streamPacks[i] = StreamPack();
        StreamPack *streamPack = &streamPacks[i];

        streamPack->avCodecParameters = avFormatContext->streams[i]->codecpar;
        streamPack->avCodec = avcodec_find_decoder(streamPack->avCodecParameters->codec_id);

        if (streamPack->avCodec == nullptr) {
            throw std::exception("Failed to find decoder.");
        }

        //Remove when audio support.
        if (streamPack->avCodecParameters->codec_type == AVMEDIA_TYPE_VIDEO) {
            videoStreamIndex = i;

            width = streamPack->avCodecParameters->width;
            height = streamPack->avCodecParameters->height;
		}
    }
    return;
}

void Video::LoadFrames(void) {
	const StreamPack *videoStream = &streamPacks[videoStreamIndex];

    AVCodecContext *avCodecContext = avcodec_alloc_context3(videoStream->avCodec);
    if (avCodecContext == nullptr) {
        throw std::exception("Failed to allocate AVCodecContext.");
    }

    if (avcodec_parameters_to_context(avCodecContext, videoStream->avCodecParameters) < 0) {
        throw std::exception("Failed to copy parameters to context.");
    }
    if (avcodec_open2(avCodecContext, videoStream->avCodec, nullptr) < 0) {
        throw std::exception("Failed to open codec.");
    }

    AVPacket *avPacket = av_packet_alloc();
    if (avPacket == nullptr) {
        throw std::exception("Failed to allocate AVPacket.");
    }

    AVFrame *avFrame = av_frame_alloc();
    if (avFrame == nullptr) {
        throw std::exception("Failed to allocate AVFrame.");
    }

    //int32_t numberOfBytes = av_image_get_buffer_size(AV_PIX_FMT_RGB24, width, height, 0);
	const int32_t numberOfBytes = avpicture_get_size(AV_PIX_FMT_RGB24, width, height);
    uint8_t *buffer = static_cast<uint8_t*>(av_malloc(numberOfBytes * sizeof(uint8_t)));

    avpicture_fill((AVPicture *)(avFrame), buffer, AV_PIX_FMT_RGB24, width, height);

    std::cout << "Reading frames." << std::endl;

    int frameFinished = 0;

    AVFrame *output = av_frame_alloc();

    struct SwsContext *swsContext = sws_getContext(width,
                                                   height,
                                                   avCodecContext->pix_fmt,
                                                   width,
                                                   height,
                                                   AV_PIX_FMT_RGB24,
                                                   SWS_BILINEAR,
                                                   nullptr,
                                                   nullptr,
                                                   nullptr);

    while (av_read_frame(avFormatContext, avPacket) >= 0) {
        avcodec_decode_video2(avCodecContext, avFrame, &frameFinished, avPacket);

        if (frameFinished > 0) {
            sws_scale(swsContext, avFrame->data, avFrame->linesize, 0, height, output->data, output->linesize);
        }


        /*
        if (avcodec_send_packet(avCodecContext, avPacket) < 0) {
            throw std::exception("Failed to send packet to the decoder.");
        }

        while (true) {
            const int32_t errorCode = avcodec_receive_frame(avCodecContext, avFrame);
            if (errorCode == AVERROR(EAGAIN) || errorCode == AVERROR_EOF) {
                break;
            }
            if (errorCode < 0) {
                throw std::exception("Failed to receive frame from the decoder.");
            }
        }
        */

        std::cout << "Frame number: " << avCodecContext->frame_number << std::endl;
        std::cin.get();
    }

    av_frame_free(&avFrame);
    av_packet_free(&avPacket);
    avcodec_free_context(&avCodecContext);
    return;
}

void Video::Dispose(void) {
    avformat_free_context(avFormatContext);
    avFormatContext = nullptr;

    free(streamPacks);
    streamPacks = nullptr;
    return;
}