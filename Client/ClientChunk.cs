using Block2D.Server;
using Microsoft.Xna.Framework;

namespace Block2D.Common
{
    public class ClientChunk : Chunk, ITickable
    {
        private const int SECTION_SIZE = CHUNK_SIZE / 4;

        public ChunkLoadAmount LoadAmount
        {
            get => _loadAmount;
        }

        public byte ReceivedSections { get; set; }

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

        public void SetSection(ushort[] tiles, byte offset)
        {
            switch (offset)
            {
                case 0:
                    for (int x = 0; x < SECTION_SIZE; x++)
                    {
                        for (int y = 0; y < SECTION_SIZE; y++)
                        {
                            Vector2 pos = new(x, y);
                            SetTile(pos, tiles[pos.ToPoint().GetIndexFromPosition()]);
                        }
                    }
                    break;
                case 1:
                    for (int x = SECTION_SIZE; x < SECTION_SIZE * 2; x++)
                    {
                        for (int y = SECTION_SIZE; y < SECTION_SIZE * 2; y++)
                        {
                            Vector2 pos = new(x, y);
                            SetTile(pos, tiles[pos.ToPoint().GetIndexFromPosition()]);
                        }
                    }
                    break;
                case 2:
                    for (int x = SECTION_SIZE * 2; x < SECTION_SIZE * 3; x++)
                    {
                        for (int y = SECTION_SIZE * 2; y < SECTION_SIZE * 3; y++)
                        {
                            Vector2 pos = new(x, y);
                            SetTile(pos, tiles[pos.ToPoint().GetIndexFromPosition()]);
                        }
                    }
                    break;
                case 3:
                    for (int x = SECTION_SIZE * 3; x < SECTION_SIZE * 4; x++)
                    {
                        for (int y = SECTION_SIZE * 3; y < SECTION_SIZE * 4; y++)
                        {
                            Vector2 pos = new(x, y);
                            SetTile(pos, tiles[pos.ToPoint().GetIndexFromPosition()]);
                        }
                    }
                    break;
            }
        }
    }
}
