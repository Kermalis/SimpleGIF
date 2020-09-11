using Kermalis.EndianBinaryIO;
using System;
using System.IO;
using System.Text;

namespace Kermalis.SimpleGIF.Decoding
{
    internal static class GifHelpers
    {
        public static void ConsumeDataBlocks(EndianBinaryReader r)
        {
            CopyDataBlocksToStream(r, Stream.Null);
        }
        public static byte[] ReadDataBlocks(EndianBinaryReader r)
        {
            using (var ms = new MemoryStream())
            {
                CopyDataBlocksToStream(r, ms);
                return ms.ToArray();
            }
        }
        public static void CopyDataBlocksToStream(EndianBinaryReader r, Stream targetStream)
        {
            int len;
            while ((len = r.ReadByte()) > 0)
            {
                targetStream.Write(r.ReadBytes(len), 0, len);
            }
        }

        public static GifColor[] ReadColorTable(EndianBinaryReader r, int size)
        {
            var colorTable = new GifColor[size];
            for (int i = 0; i < size; i++)
            {
                colorTable[i] = new GifColor(r);
            }
            return colorTable;
        }

        public static bool IsNetscapeExtension(GifApplicationExtension ext)
        {
            return ext.ApplicationIdentifier == "NETSCAPE"
                && GetString(ext.AuthenticationCode) == "2.0";
        }
        public static ushort GetRepeatCount(GifApplicationExtension ext)
        {
            if (ext.Data.Length >= 3)
            {
                return (ushort)EndianBitConverter.BytesToInt16(ext.Data, 1, Endianness.LittleEndian);
            }
            return 1;
        }

        public static Exception UnknownBlockTypeException(int blockId)
        {
            return new UnknownBlockTypeException($"Unknown block type: 0x{blockId:X2}");
        }
        public static Exception UnknownExtensionTypeException(int extensionLabel)
        {
            return new UnknownExtensionTypeException($"Unknown extension type: 0x{extensionLabel:X2}");
        }
        public static Exception InvalidBlockSizeException(string blockName, int expectedBlockSize, int actualBlockSize)
        {
            return new InvalidBlockSizeException($"Invalid block size for {blockName}. Expected {expectedBlockSize}, but was {actualBlockSize}");
        }
        public static Exception InvalidSignatureException(string signature)
        {
            return new InvalidSignatureException($"Invalid file signature: {signature}");
        }
        public static Exception UnsupportedVersionException(string version)
        {
            return new UnsupportedGifVersionException($"Unsupported version: {version}");
        }

        public static string GetString(byte[] bytes)
        {
            return GetString(bytes, 0, bytes.Length);
        }
        public static string GetString(byte[] bytes, int index, int count)
        {
            return Encoding.UTF8.GetString(bytes, index, count);
        }
    }
}
