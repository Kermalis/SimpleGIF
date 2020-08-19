using System;

namespace Kermalis.SimpleGIF.Decoding
{
    public sealed class UnknownExtensionTypeException : GifDecoderException
    {
        internal UnknownExtensionTypeException(string message) : base(message) { }
        internal UnknownExtensionTypeException(string message, Exception inner) : base(message, inner) { }
    }
}