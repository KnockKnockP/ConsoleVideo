#ifndef VIDEO_HPP
	#define VIDEO_HPP

	#include <string>
	#include <StreamPack.hpp>

	extern "C" {
		#include <libavformat/avformat.h>
	}

class Video {
		std::string filePath;

	public:
		int32_t width = -1, height = -1;

		Video(const std::string _filePath);

		~Video(void);

		void LoadNextFrame(void);

		void Dispose(void);
	};
#endif