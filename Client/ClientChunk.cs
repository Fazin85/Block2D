using Block2D.Server;
using Microsoft.Xna.Framework;

namespace Block2D.Common
{
    public class ClientChunk : Chunk, ITickable
    {
        private const int SECTION_S2 = CHUNK_SIZE / 2 * (CHUNK_SIZE / 2);
        public byte ReceivedSections { get; private set; }

        private readonly Tile[,] _tiles;
        private readonly ChunkLoadAmount _loadAmount;

        public ClientChunk(Point position)
        {
            _tiles = new Tile[CHUNK_SIZE, CHUNK_SIZE];
            _loadAmount = ChunkLoadAmount.Unloaded;
            Position = position;
            ReceivedSections = 0;
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

        public void SetSection(ushort[] blocks, byte offset)
        {
            switch (offset)
            {
                case 0:
                    for (int i = 0; i < SECTION_S2; i++)
                    {
                        Point pos = new(i / CHUNK_SIZE, i % CHUNK_SIZE);
                        SetTile(pos, blocks[i]);
                    }
                    break;
                case 1:
                    for (int i = SECTION_S2; i < SECTION_S2 * 2; i++)
                    {
                        Point pos = new(i / CHUNK_SIZE, i % CHUNK_SIZE);
                        SetTile(pos, blocks[i - SECTION_S2]);
                    }
                    break;
                case 2:
                    for (int i = SECTION_S2 * 2; i < SECTION_S2 * 3; i++)
                    {
                        Point pos = new(i / CHUNK_SIZE, i % CHUNK_SIZE);
                        SetTile(pos, blocks[i - (SECTION_S2 * 2)]);
                    }
                    break;
                case 3:
                    for (int i = SECTION_S2 * 3; i < SECTION_S2 * 4; i++)
                    {
                        Point pos = new(i / CHUNK_SIZE, i % CHUNK_SIZE);
                        SetTile(pos, blocks[i - (SECTION_S2 * 3)]);
                    }
                    break;
            }
            ReceivedSections = offset;
        }
    }
}
