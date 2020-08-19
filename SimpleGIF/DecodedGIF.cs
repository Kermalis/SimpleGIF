using System.Collections.ObjectModel;

namespace Kermalis.SimpleGIF
{
    public sealed class DecodedGIF
    {
        public sealed class Frame
        {
            public uint[] Bitmap { get; }
            /// <summary>Delay in milliseconds. -1 if it stays forever.</summary>
            public int Delay { get; }

            internal Frame(uint[] toClone, int? delay)
            {
                Bitmap = (uint[])toClone.Clone();
                if (delay.HasValue)
                {
                    int d = delay.Value;
                    Delay = d == 0 ? -1 : d;
                }
                else
                {
                    Delay = 100;
                }
            }
        }

        public ReadOnlyCollection<Frame> Frames { get; }
        public ushort RepeatCount { get; }
        public int Width { get; }
        public int Height { get; }

        internal DecodedGIF(Frame[] frames, ushort repeatCount, int width, int height)
        {
            Frames = new ReadOnlyCollection<Frame>(frames);
            RepeatCount = repeatCount;
            Width = width;
            Height = height;
        }
    }
}
