using ConsoleVideo.Math;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ConsoleVideo.Media; 

public abstract class ConsoleFrameGenerator : IFrameGenerator {
    protected static readonly char[] grayscaleCharacters = {
        ' ',
        '.',
        '|',
        '+',
        'S',
        '$',
        'L',
        '&',
        '#',
        '@'
    };

    protected static readonly int arraySize = grayscaleCharacters.Length;

    protected Vector2Int windowSize;

    protected float widthScale,
                    heightScale;

    public virtual IFrame Convert(Image<Bgr24> image) => throw new System.NotImplementedException();
}