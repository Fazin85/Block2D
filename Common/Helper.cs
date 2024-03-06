using Block2D.Server;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Block2D.Common
{
    public static class Helper
    {
        /// <summary>
        /// Compresses a byte array with Deflate compression.
        /// </summary>
        public static byte[] Compress(this byte[] data)
        {
            MemoryStream output = new();
            using (DeflateStream dstream = new(output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        /// <summary>
        /// Decompresses a byte array with Deflate decompression.
        /// </summary>
        public static byte[] Decompress(this byte[] data)
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
            return new Point { X = (point.X >> 10) * 64, Y = (point.Y >> 10) * 64 };
        }

        public static Point Abs(this Point point)
        {
            return new() { X = Math.Abs(point.X), Y = Math.Abs(point.Y) };
        }

        public static bool Collidable(this Tile tile)
        {
            return tile.ID == 0; //am I stupid or does this not make sense?
        }

        public static int GetIndexFromPosition(this Point p)
        {
            return Chunk.CHUNK_SIZE * p.X + p.Y;
        }

        public static int GetIndexFromPosition(this Vector2 p)
        {
            return (int)(Chunk.CHUNK_SIZE * p.X + p.Y);
        }

        public static int GetIndexFromPosition(int x, int y)
        {
            return (Chunk.CHUNK_SIZE * x + y);
        }

        public static Point GetRegionPos(this Chunk chunk)
        {
            return new() { X = chunk.Position.X >> 11, Y = chunk.Position.Y >> 11, };
        }

        public static byte[] ToByteArray(this ushort[] array)
        {
            byte[] target = new byte[array.Length * 2];
            Buffer.BlockCopy(array, 0, target, 0, array.Length * 2);
            return target;
        }

        public static ushort[] ToUShortArray(this byte[] array)
        {
            ushort[] target = new ushort[array.Length / 2];
            Buffer.BlockCopy(array, 0, target, 0, array.Length);
            return target;
        }

        public static void Split<T>(T[] array, int index, out T[] first, out T[] second)
        {
            first = array.Take(index).ToArray();
            second = array.Skip(index).ToArray();
        }

        public static void SplitMidPoint<T>(T[] array, out T[] first, out T[] second)
        {
            Split(array, array.Length / 2, out first, out second);
        }

        public static ushort[] GetChunkTileIds(this ServerChunk chunk)
        {
            ushort[] result = new ushort[Chunk.CHUNK_SIZE * Chunk.CHUNK_SIZE];
            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
            {
                for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                {
                    result[new Point(x, y).GetIndexFromPosition()] = chunk.Tiles[x, y].ID;
                }
            }
            return result;
        }
    }
}
