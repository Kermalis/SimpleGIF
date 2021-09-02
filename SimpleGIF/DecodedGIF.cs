using System;
using System.Collections.ObjectModel;

namespace Kermalis.SimpleGIF
{
    public sealed class DecodedGIF
    {
        public sealed class Frame
        {
            /// <summary>Modifying this <see cref="Array"/> will not affect any other frames.
            /// This is the only <see cref="object"/> exposed to changes because it doesn't affect the library.</summary>
            public uint[] Bitmap { get; }
            /// <summary>Delay in milliseconds. <see langword="null"/> if it stays forever.</summary>
            public int? Delay { get; }

            internal Frame(uint[] toClone, int? delay)
            {
                Bitmap = (uint[])toClone.Clone();
                if (delay.HasValue)
                {
                    int d = delay.Value;
                    Delay = d == 0 ? null : d;
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
