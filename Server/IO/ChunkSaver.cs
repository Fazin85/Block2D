using Block2D.Common;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;

namespace Block2D.Server.IO
{
    public static class ChunkSaver
    {
        public static void SaveChunk(ServerChunk chunk)
        {
            Point regionPos = chunk.Position.GetRegionPos();

            string path =
                chunk.Server.World.ChunkDataPath
                + "/r"
                + regionPos.X.ToString()
                + "."
                + regionPos.Y.ToString();

            string path2 =
                path
                + "/X"
                + chunk.Position.X.ToString()
                + "Y"
                + chunk.Position.Y.ToString()
                + ".chunk";

            if (File.Exists(path2))
            {
                return;
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            List<ushort> tileIds = new();

            for (int x = 0; x < CC.CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CC.CHUNK_SIZE; y++)
                {
                    var tile = chunk.GetTile(new(x, y));
                    tileIds.Add(tile.ID);
                }
            }
            byte[] compressedTiles = tileIds.ToArray().ToByteArray();

            File.WriteAllBytes(path2, compressedTiles);
        }
    }
}
