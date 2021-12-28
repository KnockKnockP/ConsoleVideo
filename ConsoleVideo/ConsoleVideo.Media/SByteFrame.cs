using ConsoleVideo.Math;

namespace ConsoleVideo.Media; 

public sealed class SByteFrame : IFrame {
    private readonly Vector2Int size;

    private readonly sbyte[] frame;

    public SByteFrame(Vector2Int _size) => (size, frame) = (_size, new sbyte[(_size.x * _size.y)]);

    public SByteFrame(Vector2Int _size, sbyte[] _chars) => (size, frame) = (_size, _chars);

    public char GetPixel(int y, int x) =>
        (char)(frame[ArrayMath.GetIndex(y,
                                        x,
                                        size.x)]);

    public void SetPixel(int y,
                         int x,
                         object value) =>
        frame[ArrayMath.GetIndex(y,
                                 x,
                                 size.x)] = (sbyte)(value);
}