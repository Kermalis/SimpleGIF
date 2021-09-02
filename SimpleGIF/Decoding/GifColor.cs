using Kermalis.EndianBinaryIO;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Kermalis.SimpleGIF.Decoding
{
    /// <summary>These tell the GifColor how to be written to the output bitmaps.
    /// Each color will take 8 bits (one byte), resulting in one uint being used.
    /// The letters are arranged in order of low to high bit significance.
    /// For example, <see cref="ARGB"/> will have A as the least significant byte and B as the most significant byte.</summary>
    public enum ColorFormat
    {
        ABGR,
        ARGB,
        BGRA,
        RGBA,
    }
    public sealed class GifPalette
    {
        /// <summary><see langword="null"/> if there is no transparency in this <see cref="GifPalette"/>.</summary>
        public int? TransparencyIndex { get; }
        public ReadOnlyCollection<GifColor> Colors { get; }

        internal GifPalette(int? transparencyIndex, GifColor[] colors)
        {
            TransparencyIndex = transparencyIndex;
            Colors = new ReadOnlyCollection<GifColor>(colors);
        }
    }
    public struct GifColor
    {
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }

        internal GifColor(EndianBinaryReader r)
        {
            R = r.ReadByte();
            G = r.ReadByte();
            B = r.ReadByte();
        }

        public uint ToColorFormat(ColorFormat f)
        {
            switch (f)
            {
                case ColorFormat.ABGR: return ToABGR();
                case ColorFormat.ARGB: return ToARGB();
                case ColorFormat.BGRA: return ToBGRA();
                case ColorFormat.RGBA: return ToRGBA();
                default: throw new ArgumentOutOfRangeException(nameof(f));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ToABGR()
        {
            return ((uint)R << 24) | ((uint)G << 16) | ((uint)B << 8) | 0xFF;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ToARGB()
        {
            return ((uint)B << 24) | ((uint)G << 16) | ((uint)R << 8) | 0xFF;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ToBGRA()
        {
            return ((uint)0xFF << 24) | ((uint)R << 16) | ((uint)G << 8) | B;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ToRGBA()
        {
            return ((uint)0xFF << 24) | ((uint)B << 16) | ((uint)G << 8) | R;
        }

        public override string ToString()
        {
            return $"#{R:X2}{G:X2}{B:X2}";
        }
    }
}
