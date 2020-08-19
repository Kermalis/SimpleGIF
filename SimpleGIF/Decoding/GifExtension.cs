using Kermalis.EndianBinaryIO;
using System.Collections.Generic;

namespace Kermalis.SimpleGIF.Decoding
{
    public abstract class GifExtension : GifBlock
    {
        internal const int ExtensionIntroducer = 0x21;

        internal static new GifExtension Read(EndianBinaryReader r, IEnumerable<GifExtension> controlExtensions)
        {
            byte label = r.ReadByte();
            switch (label)
            {
                case GifGraphicControlExtension.ExtensionLabel: return new GifGraphicControlExtension(r);
                case GifCommentExtension.ExtensionLabel: return new GifCommentExtension(r);
                case GifPlainTextExtension.ExtensionLabel: return new GifPlainTextExtension(r, controlExtensions);
                case GifApplicationExtension.ExtensionLabel: return new GifApplicationExtension(r);
                default: throw GifHelpers.UnknownExtensionTypeException(label);
            }
        }
    }
}
