using Block2D.Common;

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

        public void GenerateChunkTerrain(Chunk chunk)
        {
            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
            {
                for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                {
                    chunk.SetTile(new(x, y), 1);
                }
            }
        }
    }
}
