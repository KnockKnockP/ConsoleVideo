using ConsoleVideo.Math;

namespace ConsoleVideo.Media {
    public sealed class CharFrame : IFrame<char> {
        private readonly char[] frame;

        public Vector2Int Size {
            get;
        }

        public CharFrame(Vector2Int _size) => (Size, frame) = (_size, new char[(_size.x * _size.y)]);

        public CharFrame(Vector2Int _size, char[] _chars) => (Size, frame) = (_size, _chars);

        public char GetPixel(int index) => frame[index];

        public char GetPixel(int y, int x) =>
            frame[ArrayMath.GetIndex(y,
                                     x,
                                     Size.x)];

        public void SetPixel(int index, char value) => frame[index] = value;

        public void SetPixel(int y,
                             int x,
                             char value) =>
            frame[ArrayMath.GetIndex(y,
                                     x,
                                     Size.x)] = value;
    }
}