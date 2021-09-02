using Kermalis.EndianBinaryIO;
using Kermalis.SimpleGIF.Decoding;
using Kermalis.SimpleGIF.Decompression;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kermalis.SimpleGIF
{
    public static class GIFRenderer
    {
        public struct Int32Rect
        {
            public int X { get; }
            public int Y { get; }
            public int Width { get; }
            public int Height { get; }

            public Int32Rect(int x, int y, int width, int height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }
        }

        public static EndianBinaryReader GetReader(Stream stream)
        {
            return new EndianBinaryReader(stream, encoding: EncodingType.UTF8);
        }
        public static GifDataStream GetGif(Stream stream)
        {
            using (EndianBinaryReader r = GetReader(stream))
            {
                return GetGif(r);
            }
        }
        public static GifDataStream GetGif(EndianBinaryReader r)
        {
            return new GifDataStream(r);
        }
        public static void GetGif(Stream stream, out GifDataStream gif, out List<byte>[] decompressedFrames)
        {
            using (EndianBinaryReader r = GetReader(stream))
            {
                gif = GetGif(r);
                decompressedFrames = DecompressAllFrames(r, gif);
            }
        }
        public static void GetGif(EndianBinaryReader r, out GifDataStream gif, out List<byte>[] decompressedFrames)
        {
            gif = GetGif(r);
            decompressedFrames = DecompressAllFrames(r, gif);
        }

        public static List<byte>[] DecompressAllFrames(EndianBinaryReader r, GifDataStream gif)
        {
            var ret = new List<byte>[gif.Frames.Count];
            for (int i = 0; i < gif.Frames.Count; i++)
            {
                GifFrame frame = gif.Frames[i];
                r.BaseStream.Position = frame.ImageData.CompressedDataStartOffset;
                using (var ms = new MemoryStream())
                {
                    GifHelpers.CopyDataBlocksToStream(r, ms);
                    var lzwStream = new LzwDecompressStream(ms.GetBuffer(), frame.ImageData.LzwMinimumCodeSize);
                    ret[i] = lzwStream.Convert();
                }
            }
            return ret;
        }
        public static DecodedGIF DecodeAllFrames(Stream stream, ColorFormat colorFormat)
        {
            GetGif(stream, out GifDataStream gif, out List<byte>[] decompressedFrames);
            return RenderFrames(gif, decompressedFrames, colorFormat);
        }

        public static GifPalette[] CreatePalettes(GifDataStream gif)
        {
            var palettes = new GifPalette[gif.Frames.Count];
            GifColor[]? globalColorTable = gif.GlobalColorTable;

            for (int i = 0; i < gif.Frames.Count; i++)
            {
                GifFrame frame = gif.Frames[i];
                GifColor[] colorTable = frame.LocalColorTable ?? globalColorTable!;

                palettes[i] = new GifPalette(frame.GraphicControl?.TransparencyIndex, colorTable);
            }

            return palettes;
        }

        public static unsafe DecodedGIF.Frame RenderFrame(GifDataStream gif, IReadOnlyList<IReadOnlyList<byte>> decompressedFrames, GifPalette[] pals, int frameIndex,
            int canvasWidth, int canvasHeight, ColorFormat colorFormat, ref uint[] canvas, ref GifFrame? prevFrame, ref int prevFrameIndex, ref uint[] prevFrameBuffer)
        {
            GifFrame frame = gif.Frames[frameIndex];
            GifImageDescriptor desc = frame.Descriptor;
            Int32Rect rect = GetFixedUpFrameRect(canvasWidth, canvasHeight, desc);
            DisposePreviousFrame(frame, prevFrame, canvasWidth, canvasHeight, ref canvas, ref prevFrameBuffer);

            IReadOnlyList<byte> indexBuffer = decompressedFrames[frameIndex];
            int indexBufferIndex = 0;

            GifPalette palette = pals[frameIndex];

            IEnumerable<int> rows = desc.Interlace ? InterlacedRows(rect.Height) : NormalRows(rect.Height);
            fixed (uint* bmpAddress = canvas)
            {
                foreach (int y in rows)
                {
                    for (int x = 0; x < rect.Width; x++)
                    {
                        byte colorIndex = indexBuffer[indexBufferIndex++];
                        if (colorIndex != palette.TransparencyIndex)
                        {
                            GifColor c = palette.Colors[colorIndex];
                            *GetPixelAddress(bmpAddress, canvasWidth, x + desc.Left, y + desc.Top) = c.ToColorFormat(colorFormat);
                        }
                    }
                }
            }
            prevFrameIndex = frameIndex;
            prevFrame = frame;

            return new DecodedGIF.Frame(canvas, frame.GraphicControl?.Delay);
        }
        public static DecodedGIF RenderFrames(GifDataStream gif, IReadOnlyList<IReadOnlyList<byte>> decompressedFrames, ColorFormat colorFormat)
        {
            GifLogicalScreenDescriptor gifDesc = gif.Header.LogicalScreenDescriptor;
            int canvasWidth = gifDesc.Width;
            int canvasHeight = gifDesc.Height;
            uint[] canvas = new uint[canvasWidth * canvasHeight];
            GifPalette[] pals = CreatePalettes(gif);
            int prevFrameIndex = 0;
            GifFrame? prevFrame = null;
            uint[] prevFrameBuffer = new uint[canvasWidth * canvasHeight];
            var outSprites = new DecodedGIF.Frame[gif.Frames.Count];
            for (int frameIndex = 0; frameIndex < gif.Frames.Count; frameIndex++)
            {
                outSprites[frameIndex] = RenderFrame(gif, decompressedFrames, pals, frameIndex, canvasWidth, canvasHeight, colorFormat, ref canvas, ref prevFrame, ref prevFrameIndex, ref prevFrameBuffer);
            }
            return new DecodedGIF(outSprites, gif.RepeatCount, canvasWidth, canvasHeight);
        }

        public static IEnumerable<int> NormalRows(int height)
        {
            return Enumerable.Range(0, height);
        }
        private static readonly (int Start, int Step)[] _interlacePasses = new[]
        {
            (0, 8),
            (4, 8),
            (2, 4),
            (1, 2),
        };
        public static IEnumerable<int> InterlacedRows(int height)
        {
            /*
             * 4 passes:
             * Pass 1: rows 0, 8, 16, 24...
             * Pass 2: rows 4, 12, 20, 28...
             * Pass 3: rows 2, 6, 10, 14...
             * Pass 4: rows 1, 3, 5, 7...
             * */
            foreach ((int start, int step) in _interlacePasses)
            {
                int y = start;
                while (y < height)
                {
                    yield return y;
                    y += step;
                }
            }
        }
        public static Int32Rect GetFixedUpFrameRect(int canvasWidth, int canvasHeight, IGifRect r)
        {
            return new Int32Rect(r.Left, r.Top, Math.Min(r.Width, canvasWidth - r.Left), Math.Min(r.Height, canvasHeight - r.Top));
        }
        public static unsafe void DisposePreviousFrame(GifFrame currentFrame, GifFrame? prevFrame, int canvasWidth, int canvasHeight, ref uint[] canvas, ref uint[] prevFrameBuffer)
        {
            GifGraphicControlExtension? pgce = prevFrame?.GraphicControl;
            if (pgce is not null)
            {
                switch (pgce.DisposalMethod)
                {
                    case GifFrameDisposalMethod.None:
                    case GifFrameDisposalMethod.DoNotDispose:
                    {
                        // Leave previous frame in place
                        break;
                    }
                    case GifFrameDisposalMethod.RestoreBackground:
                    {
                        fixed (uint* bmpAddress = canvas)
                        {
                            ClearArea(bmpAddress, canvasWidth, canvasHeight, GetFixedUpFrameRect(canvasWidth, canvasHeight, prevFrame!.Descriptor));
                        }
                        break;
                    }
                    case GifFrameDisposalMethod.RestorePrevious:
                    {
                        Buffer.BlockCopy(prevFrameBuffer, 0, canvas, 0, canvas.Length * sizeof(uint));
                        break;
                    }
                }
            }

            GifGraphicControlExtension? gce = currentFrame.GraphicControl;
            if (gce is not null && gce.DisposalMethod == GifFrameDisposalMethod.RestorePrevious)
            {
                Buffer.BlockCopy(canvas, 0, prevFrameBuffer, 0, canvas.Length * sizeof(uint));
            }
        }
        private static unsafe void ClearArea(uint* bmpAddress, int bmpWidth, int bmpHeight, IGifRect rect)
        {
            ClearArea(bmpAddress, bmpWidth, bmpHeight, rect.Left, rect.Top, rect.Width, rect.Height);
        }
        private static unsafe void ClearArea(uint* bmpAddress, int bmpWidth, int bmpHeight, Int32Rect rect)
        {
            ClearArea(bmpAddress, bmpWidth, bmpHeight, rect.X, rect.Y, rect.Width, rect.Height);
        }
        private static unsafe void ClearArea(uint* bmpAddress, int bmpWidth, int bmpHeight, int x, int y, int width, int height)
        {
            for (int py = y; py < y + height; py++)
            {
                for (int px = x; px < x + width; px++)
                {
                    *GetPixelAddress(bmpAddress, bmpWidth, px, py) = 0;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe uint* GetPixelAddress(uint* bmpAddress, int bmpWidth, int x, int y)
        {
            return bmpAddress + x + (y * bmpWidth);
        }
    }
}
