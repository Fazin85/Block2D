﻿using Microsoft.Xna.Framework;
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

        public static Point ToChunkCoords(this Point point)
        {
            return new Point
            {
                X = (point.X >> 10) * 64,
                Y = (point.Y >> 10) * 64
            };
        }
        public static Point Abs(this Point point)
        {
            return new()
            {
                X = Math.Abs(point.X),
                Y = Math.Abs(point.Y)
            };
        }

        public static bool Collidable(this Tile tile)
        {
            return tile.ID == 0;//am I stupid or does this not make sense?
        }
    }
}
