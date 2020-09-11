using Kermalis.EndianBinaryIO;

namespace Kermalis.SimpleGIF.Decoding
{
    public sealed class GifHeader : GifBlock
    {
        internal override GifBlockKind Kind => GifBlockKind.Other;

        public string Signature { get; }
        public string Version { get; }
        public GifLogicalScreenDescriptor LogicalScreenDescriptor { get; }

        internal GifHeader(EndianBinaryReader r)
        {
            Signature = r.ReadString(3, false);
            if (Signature != "GIF")
            {
                throw GifHelpers.InvalidSignatureException(Signature);
            }
            Version = r.ReadString(3, false);
            if (Version != "87a" && Version != "89a")
            {
                throw GifHelpers.UnsupportedVersionException(Version);
            }
            LogicalScreenDescriptor = new GifLogicalScreenDescriptor(r);
        }
    }
}
