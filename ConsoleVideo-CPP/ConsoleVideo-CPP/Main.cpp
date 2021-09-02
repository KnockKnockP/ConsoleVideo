#include <Console.hpp>
#include <ExitCode.hpp>
#include <iostream>
#include <Video.hpp>

extern "C" {
    #include <libavformat/avformat.h>
    #include <libavcodec/avcodec.h>
}

void FFmpegTest(char *path) {
    AVFormatContext *avFormatContext = avformat_alloc_context();
    if (avFormatContext == nullptr) {
        throw std::exception("Failed to allocate AVFormatContext.");
    }

    if (avformat_open_input(&avFormatContext, path, nullptr, nullptr) != 0) {
	    const std::string message("Failed to open file at path " + std::string(path));
        throw std::exception(message.c_str());
    }

    std::cout << "Format: " << avFormatContext->iformat->long_name << std::endl <<
                 "Duration: " << avFormatContext->duration << std::endl;

    if (avformat_find_stream_info(avFormatContext, nullptr) < 0) {
        throw std::exception("Failed to find stream information.");
    }

    for (uint32_t i = 0; i < avFormatContext->nb_streams; ++i) {
	    const AVCodecParameters *avCodecParameters = avFormatContext->streams[i]->codecpar;
        AVCodec *avCodec = avcodec_find_decoder(avCodecParameters->codec_id);
	    if (avCodec == nullptr) {
            throw std::exception("Failed to find decoder.");
        }

        std::cout << "Codec: " << avCodec->long_name << std::endl <<
                     "ID: " << avCodec->id << std::endl <<
                     "Bit rate: " << avCodecParameters->bit_rate << std::endl;

        if (avCodecParameters->codec_type == AVMEDIA_TYPE_VIDEO) {
            std::cout << "Video codec:" << std::endl <<
                         "    Resolution: (" << avCodecParameters->width << ", " << avCodecParameters->height << ")" << std::endl;
        } else if (avCodecParameters->codec_type == AVMEDIA_TYPE_AUDIO) {
            std::cout << "Audio codec:" << std::endl <<
                         "    Channel count: " << avCodecParameters->channels << std::endl <<
                         "    Sample rate: " << avCodecParameters->sample_rate << std::endl;
            continue;
        }

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
    }

    avformat_free_context(avFormatContext);
    return;
}

int main(int argc, char *argv[]) {
    ExitCode exitCode = Success;
    
    try {
        const Console console(L"ConsoleVideo");
        console.SetupConsoleRestrictions();

        Video video(/*argv[1]*/ "D:\\GitHub\\ConsoleVideo\\ConsoleVideo\\ConsoleVideo\\Resources\\Videos\\TestVideo.mp4");
        video.LoadCodec();

        std::cout << "(" << video.width << ", " << video.height << ")" << std::endl;

        console.RevertConsoleRestrictions();
    } catch (const std::exception& exception) {
        std::cerr << "Exception: " << exception.what() << std::endl;
        exitCode = Failure;
    }
    
    std::cout << "Execution done." << std::endl <<
                 "Exit code: " << exitCode << std::endl;
    std::cin.get();
    return exitCode;
}