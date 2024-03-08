using Block2D.Common;
using Microsoft.Xna.Framework;
using System;

namespace Block2D.Server.WorldGen
{
    public class TerrainGenerator
    {
        private readonly string _dimensionToGenerate;
        private readonly int _seed;

        public TerrainGenerator(string dimensionToGenerate, int seed)
        {
            _dimensionToGenerate = dimensionToGenerate;
            _seed = seed;
        }

        public void GenerateChunk(Point chunkPosition, out ServerChunk chunk)
        {
            chunk = new(chunkPosition);

            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
            {
                for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                {
                    if (Random.Shared.Next(2) == 0)
                    {
                        chunk.SetTile(new(x, y), 1);
                    }
                    else if (Random.Shared.Next(2) == 0)
                    {
                        chunk.SetTile(new(x, y), 3);
                    }
                    else
                    {
                        chunk.SetTile(new(x, y), 2);
                    }
                }
            }
        }
    }
}
