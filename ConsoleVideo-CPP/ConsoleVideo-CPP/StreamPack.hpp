#ifndef STREAM_PACK_HPP
	#define STREAM_PACK_HPP

	extern "C" {
		#include <libavformat/avformat.h>
		#include <libavcodec/avcodec.h>
	}

	typedef struct StreamPack_ {
		AVCodecParameters *avCodecParameters;
		AVCodec *avCodec;
	} StreamPack;
#endif