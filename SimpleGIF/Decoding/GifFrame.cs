using Kermalis.EndianBinaryIO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kermalis.SimpleGIF.Decoding
{
    public sealed class GifFrame : GifBlock
    {
        internal const int ImageSeparator = 0x2C;
        internal override GifBlockKind Kind => GifBlockKind.GraphicRendering;

        public GifImageDescriptor Descriptor { get; }
        public GifColor[]? LocalColorTable { get; }
        public ReadOnlyCollection<GifExtension> Extensions { get; }
        public GifImageData ImageData { get; }
        public GifGraphicControlExtension? GraphicControl { get; }

        internal GifFrame(EndianBinaryReader r, IEnumerable<GifExtension> controlExtensions)
        {
            Descriptor = new GifImageDescriptor(r);
            if (Descriptor.HasLocalColorTable)
            {
                LocalColorTable = GifHelpers.ReadColorTable(r, Descriptor.LocalColorTableSize);
            }
            ImageData = new GifImageData(r);
            Extensions = controlExtensions.ToList().AsReadOnly();
            GraphicControl = Extensions.OfType<GifGraphicControlExtension>().LastOrDefault();
        }
    }
}
