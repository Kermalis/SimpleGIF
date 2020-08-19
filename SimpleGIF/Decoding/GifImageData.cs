using Kermalis.EndianBinaryIO;

namespace Kermalis.SimpleGIF.Decoding
{
    public sealed class GifImageData
    {
        public byte LzwMinimumCodeSize { get; }
        public long CompressedDataStartOffset { get; }

        internal GifImageData(EndianBinaryReader r)
        {
            LzwMinimumCodeSize = r.ReadByte();
            CompressedDataStartOffset = r.BaseStream.Position;
            GifHelpers.ConsumeDataBlocks(r);
        }
    }
}
