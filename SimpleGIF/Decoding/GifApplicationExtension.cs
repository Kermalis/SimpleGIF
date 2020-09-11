using Kermalis.EndianBinaryIO;

namespace Kermalis.SimpleGIF.Decoding
{
    public sealed class GifApplicationExtension : GifExtension
    {
        internal const int ExtensionLabel = 0xFF;
        internal override GifBlockKind Kind => GifBlockKind.SpecialPurpose;

        public int BlockSize { get; }
        public string ApplicationIdentifier { get; }
        public byte[] AuthenticationCode { get; }
        public byte[] Data { get; }

        internal GifApplicationExtension(EndianBinaryReader r)
        {
            BlockSize = r.ReadByte(); // Should always be 11
            if (BlockSize != 11)
            {
                throw GifHelpers.InvalidBlockSizeException("Application Extension", 11, BlockSize);
            }
            ApplicationIdentifier = r.ReadString(8, true);
            AuthenticationCode = r.ReadBytes(3);
            Data = GifHelpers.ReadDataBlocks(r);
        }
    }
}
