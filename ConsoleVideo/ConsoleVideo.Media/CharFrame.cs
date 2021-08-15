using ConsoleVideo.Math;

namespace ConsoleVideo.Media {
    public sealed class CharFrame : IFrame {
        private readonly Vector2Int size;

        private readonly char[] frame;

        public CharFrame(Vector2Int _size) => (size, frame) = (_size, new char[(_size.x * _size.y)]);

        public CharFrame(Vector2Int _size, char[] _chars) => (size, frame) = (_size, _chars);

        public char GetPixel(int y, int x) =>
            frame[ArrayMath.GetIndex(y,
                                     x,
                                     size.x)];

        public void SetPixel(int y,
                             int x,
                             object value) =>
            frame[ArrayMath.GetIndex(y,
                                     x,
                                     size.x)] = (char)(value);
    }
}