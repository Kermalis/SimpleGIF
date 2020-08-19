using System;

namespace Kermalis.SimpleGIF.Decoding
{
    public sealed class InvalidBlockSizeException : GifDecoderException
    {
        internal InvalidBlockSizeException(string message) : base(message) { }
        internal InvalidBlockSizeException(string message, Exception inner) : base(message, inner) { }
    }
}