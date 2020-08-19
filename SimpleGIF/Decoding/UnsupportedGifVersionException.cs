using System;

namespace Kermalis.SimpleGIF.Decoding
{
    public sealed class UnsupportedGifVersionException : GifDecoderException
    {
        internal UnsupportedGifVersionException(string message) : base(message) { }
        internal UnsupportedGifVersionException(string message, Exception inner) : base(message, inner) { }
    }
}