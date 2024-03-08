using Block2D.Common;
using Microsoft.Xna.Framework;

namespace Block2D.Server
{
    public class ServerChunk : Chunk, ITickable
    {
        public ChunkLoadAmount LoadAmount
        {
            get => _loadAmount;
        }

        public ChunkSectionData[] Sections
        {
            get => ChunkSectionData.GetChunkSections(this);
        }

        private readonly ChunkLoadAmount _loadAmount;
        private readonly Tile[,] _tiles;

        public ServerChunk(Point position)
        {
            _tiles = new Tile[CHUNK_SIZE, CHUNK_SIZE];
            _loadAmount = ChunkLoadAmount.Unloaded;
            Position = position;
        }

        public void Tick()
        {
            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    _tiles[x, y].Tick();
                }
            }
        }
    }
}
