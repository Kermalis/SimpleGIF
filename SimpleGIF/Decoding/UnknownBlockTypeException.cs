using System;

namespace Kermalis.SimpleGIF.Decoding
{
    public sealed class UnknownBlockTypeException : GifDecoderException
    {
        internal UnknownBlockTypeException(string message) : base(message) { }
        internal UnknownBlockTypeException(string message, Exception inner) : base(message, inner) { }
    }
}