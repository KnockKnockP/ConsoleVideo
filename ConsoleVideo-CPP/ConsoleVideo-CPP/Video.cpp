#include <Video.hpp>

extern "C" {
    #include <libavformat/avformat.h>
    #include <libavcodec/avcodec.h>
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
            width = streamPack->avCodecParameters->width;
            height = streamPack->avCodecParameters->height;
		}

        /*
        AVCodecContext *avCodecContext = avcodec_alloc_context3(avCodec);
        if (avCodecContext == nullptr) {
            throw std::exception("Failed to allocate AVCodecContext.");
        }

        if (avcodec_parameters_to_context(avCodecContext, avCodecParameters) < 0) {
            throw std::exception("Failed to copy parameters to context.");
        }
        if (avcodec_open2(avCodecContext, avCodec, nullptr) < 0) {
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

        while (av_read_frame(avFormatContext, avPacket) >= 0) {
            //int response = decode_packet

            if (avcodec_send_packet(avCodecContext, avPacket) != 0) {
                throw std::exception("Failed to send packet to the decoder.");
            }

            if (avcodec_receive_frame(avCodecContext, avFrame) != 0) {
                throw std::exception("Failed to receive frame from the decoder.");
            }

            std::cout << "Frame number: " << avCodecContext->frame_number;
            std::cin.get();
        }

        av_frame_free(&avFrame);
        av_packet_free(&avPacket);
        avcodec_free_context(&avCodecContext);
        */
    }
    return;
}

void Video::Dispose(void) {
    avformat_free_context(avFormatContext);
    avFormatContext = nullptr;

    free(streamPacks);
    streamPacks = nullptr;
    return;
}