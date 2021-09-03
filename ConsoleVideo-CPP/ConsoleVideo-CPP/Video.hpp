#ifndef VIDEO_HPP
	#define VIDEO_HPP

	#include <string>
	#include <StreamPack.hpp>

	extern "C" {
		#include <libavformat/avformat.h>
	}

class Video {
		std::string filePath;
		AVFormatContext *avFormatContext = nullptr;
		StreamPack *streamPacks = nullptr;

		uint32_t videoStreamIndex = -1;

	public:
		int32_t width = -1, height = -1;

		Video(const std::string _filePath);

		~Video(void);

		void LoadCodec(void);

		void LoadFrames(void);

		void Dispose(void);
	};
#endif