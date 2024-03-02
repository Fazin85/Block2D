using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.IO.Compression;

namespace Block2D.Common
{
    public static class Helper
    {
        public static byte[] Compress(byte[] data)
        {
            MemoryStream output = new();
            using (DeflateStream dstream = new(output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        public static byte[] Decompress(byte[] data)
        {
            MemoryStream input = new(data);
            MemoryStream output = new();
            using (DeflateStream dstream = new(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }

        public static Vector2 ToChunkCoords(this Vector2 pos)
        {
            return new Vector2
            {
                X = (int)pos.X >> 7,
                Y = (int)pos.Y >> 7,
            };
        }
    }
}
