using Kermalis.EndianBinaryIO;

namespace Kermalis.SimpleGIF.Decoding
{
    public sealed class GifGraphicControlExtension : GifExtension
    {
        internal const int ExtensionLabel = 0xF9;
        internal override GifBlockKind Kind => GifBlockKind.Control;

        public int BlockSize { get; }
        public GifFrameDisposalMethod DisposalMethod { get; }
        public bool UserInput { get; }
        public int Delay { get; }
        /// <summary><see langword="null"/> if there is no transparency.</summary>
        public int? TransparencyIndex { get; }

        internal GifGraphicControlExtension(EndianBinaryReader r)
        {
            BlockSize = r.ReadByte(); // Should always be 4
            if (BlockSize != 4)
            {
                throw GifHelpers.InvalidBlockSizeException("Graphic Control Extension", 4, BlockSize);
            }
            byte packedFields = r.ReadByte();
            DisposalMethod = (GifFrameDisposalMethod)((packedFields & 0x1C) >> 2);
            UserInput = (packedFields & 0x02) != 0;
            bool hasTransparency = (packedFields & 0x01) != 0;
            Delay = r.ReadUInt16() * 10;
            TransparencyIndex = r.ReadByte(); // Still consume byte even if it won't be used
            if (!hasTransparency)
            {
                TransparencyIndex = null;
            }
            r.ReadByte(); // Block terminator
        }
    }
}
