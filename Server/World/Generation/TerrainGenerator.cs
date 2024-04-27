using Block2D.Common;
using Block2D.Common.ID;
using Microsoft.Xna.Framework;
using Simplex;

namespace Block2D.Server.World.Generation
{
    public class TerrainGenerator
    {
        private readonly string _dimensionToGenerate;
        private readonly int _seed;
        private readonly Noise Noise;
        private readonly InternalServer _internalServer;

        public TerrainGenerator(string dimensionToGenerate, int seed, InternalServer internalServer)
        {
            _dimensionToGenerate = dimensionToGenerate;
            _seed = seed;
            Noise = new() { Seed = seed };
            _internalServer = internalServer;
        }

        public void GenerateChunk(
            Point chunkPosition,
            string dimensionId,
            out ServerChunk chunk
        )
        {
            chunk = new(chunkPosition, dimensionId, _internalServer);

            if (chunkPosition.Y == 0)
            {
                for (int x = chunkPosition.X; x < chunkPosition.X + CC.CHUNK_SIZE; x++)
                {
                    int height = (int)Noise.CalcPixel1D(x, 0.1f) / 16;

                    for (int y = 0; y < CC.CHUNK_SIZE; y++)
                    {
                        Point position = new(x - chunkPosition.X, y);
                        if (y > height)
                        {
                            chunk.SetTile(position, TileID.DIRT);
                        }
                        else
                        {
                            chunk.SetTile(position, TileID.AIR);
                        }
                    }
                }
            }

            chunk.LoadAmount = ChunkLoadAmount.FullyLoaded; //do this for now
        }
    }
}
