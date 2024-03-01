namespace Block2D.Server.WorldGen
{
    public class TerrainGenerator
    {
        private int _seed;
        
        public TerrainGenerator(int seed)
        {
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
