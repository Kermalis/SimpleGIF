using Kermalis.EndianBinaryIO;
using System.Collections.Generic;

namespace Kermalis.SimpleGIF.Decoding
{
    internal enum GifBlockKind
    {
        Control,
        GraphicRendering,
        SpecialPurpose,
        Other
    }
    public abstract class GifBlock
    {
        internal abstract GifBlockKind Kind { get; }

        internal static GifBlock Read(EndianBinaryReader r, IEnumerable<GifExtension> controlExtensions)
        {
            byte blockId = r.ReadByte();
            switch (blockId)
            {
                case GifExtension.ExtensionIntroducer: return GifExtension.Read(r, controlExtensions);
                case GifFrame.ImageSeparator: return new GifFrame(r, controlExtensions);
                case GifTrailer.TrailerByte: return new GifTrailer();
                default: throw GifHelpers.UnknownBlockTypeException(blockId);
            }
        }
    }
}
