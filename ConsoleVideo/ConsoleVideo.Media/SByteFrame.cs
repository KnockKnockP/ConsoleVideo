using ConsoleVideo.Math;

namespace ConsoleVideo.Media {
    public sealed class SByteFrame : IFrame<sbyte> {
        private readonly Vector2Int size;

        private readonly sbyte[] frame;

        public SByteFrame(Vector2Int _size) => (size, frame) = (_size, new sbyte[(_size.x * _size.y)]);

        public SByteFrame(Vector2Int _size, sbyte[] _chars) => (size, frame) = (_size, _chars);

        public sbyte GetPixel(int index) => frame[index];

        public sbyte GetPixel(int y, int x) =>
            frame[ArrayMath.GetIndex(y,
                                     x,
                                     size.x)];

        public void SetPixel(int index, sbyte value) => frame[index] = value;

        public void SetPixel(int y,
                             int x,
                             sbyte value) =>
            frame[ArrayMath.GetIndex(y,
                                     x,
                                     size.x)] = value;
    }
}