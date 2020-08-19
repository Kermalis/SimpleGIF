namespace Kermalis.SimpleGIF.Decoding
{
    public interface IGifRect
    {
        int Left { get; }
        int Top { get; }
        int Width { get; }
        int Height { get; }
    }
}