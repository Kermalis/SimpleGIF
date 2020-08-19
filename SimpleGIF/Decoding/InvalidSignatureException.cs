﻿using System;

namespace Kermalis.SimpleGIF.Decoding
{
    public sealed class InvalidSignatureException : GifDecoderException
    {
        internal InvalidSignatureException(string message) : base(message) { }
        internal InvalidSignatureException(string message, Exception inner) : base(message, inner) { }
    }
}