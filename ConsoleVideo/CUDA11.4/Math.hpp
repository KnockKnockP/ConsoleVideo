#ifndef MATH_HPP
	#define MATH_HPP

	#include <cstdint>

	/*
		cstdint
	*/

	namespace ConsoleVideo::Math{
		public ref struct Vector2Int {
			int32_t x, y;

		public:
			Vector2Int(const int32_t _x, const int32_t _y) : x(_x), y(_y) {
				return;
			}
		};
	}
#endif