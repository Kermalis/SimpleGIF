namespace Kermalis.SimpleGIF.Decoding
{
    public sealed class GifTrailer : GifBlock
    {
        internal const int TrailerByte = 0x3B;
        internal override GifBlockKind Kind => GifBlockKind.Other;

        internal GifTrailer() { }
    }
}
