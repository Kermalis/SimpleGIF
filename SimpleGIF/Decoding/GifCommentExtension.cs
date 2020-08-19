using Kermalis.EndianBinaryIO;

namespace Kermalis.SimpleGIF.Decoding
{
    public sealed class GifCommentExtension : GifExtension
    {
        internal const int ExtensionLabel = 0xFE;
        internal override GifBlockKind Kind => GifBlockKind.SpecialPurpose;

        public string Text { get; }

        internal GifCommentExtension(EndianBinaryReader r)
        {
            Text = GifHelpers.GetString(GifHelpers.ReadDataBlocks(r));
        }
    }
}
