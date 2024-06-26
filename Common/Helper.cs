﻿using Block2D.Server.World;
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
            return new Point { X = (point.X >> 6) * 64, Y = (point.Y >> 6) * 64 };
        }

        public static int GetIndexFromPosition(this Point p)
        {
            return CC.CHUNK_SIZE * p.X + p.Y;
        }

        public static int GetIndexFromPosition(int x, int y)
        {
            return CC.CHUNK_SIZE * x + y;
        }

        public static Point GetRegionPos(this Point chunkPos)
        {
            return new() { X = chunkPos.X >> 11, Y = chunkPos.Y >> 11, };
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
            ushort[] result = new ushort[CC.CHUNK_SIZE * CC.CHUNK_SIZE];
            for (int x = 0; x < CC.CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CC.CHUNK_SIZE; y++)
                {
                    result[new Point(x, y).GetIndexFromPosition()] = chunk.GetTile(new(x, y)).ID;
                }
            }
            return result;
        }

        public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        public static Vector2 GetIntersectionDepth(this Rectangle rectA, Rectangle rectB)
        {
            // Calculate half sizes.
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            // Calculate centers.
            Vector2 centerA = new(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            Vector2 centerB = new(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            // Calculate and return intersection depths.
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }

        public static Vector2 Parse(string str)
        {
            int index = str.IndexOf(' ');
            string x = str.Remove(index, str.Length - index);
            string y = str.Remove(0, x.Length);
            return new Vector2(int.Parse(x), int.Parse(y));
        }
    }
}
