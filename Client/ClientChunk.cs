using Microsoft.Xna.Framework;

namespace Block2D.Common
{
    public class ClientChunk : Chunk
    {
        private const int SECTION_SIZE = CHUNK_SIZE * CHUNK_SIZE / CHUNK_SECTION_COUNT;
        public byte ReceivedSections { get; private set; }

        public ClientChunk(Point position, World world)
            : base(world)
        {
            Position = position;
            ReceivedSections = 0;
        }

        public void SetSection(ushort[] blocks, byte offset)
        {
            switch (offset)
            {
                case 0:
                    for (int i = 0; i < SECTION_SIZE; i++)
                    {
                        Point pos = new(i / CHUNK_SIZE, i % CHUNK_SIZE);

                        string tileName = World.GetTileName(blocks[i]);
                        SetTile(pos, tileName);
                    }
                    break;
                case 1:
                    for (int i = SECTION_SIZE; i < SECTION_SIZE * 2; i++)
                    {
                        Point pos = new(i / CHUNK_SIZE, i % CHUNK_SIZE);
                        string tileName = World.GetTileName(blocks[i - SECTION_SIZE]);
                        SetTile(pos, tileName);
                    }
                    break;
                case 2:
                    for (int i = SECTION_SIZE * 2; i < SECTION_SIZE * 3; i++)
                    {
                        Point pos = new(i / CHUNK_SIZE, i % CHUNK_SIZE);
                        string tileName = World.GetTileName(blocks[i - (SECTION_SIZE * 2)]);
                        SetTile(pos, tileName);
                    }
                    break;
                case 3:
                    for (int i = SECTION_SIZE * 3; i < SECTION_SIZE * 4; i++)
                    {
                        Point pos = new(i / CHUNK_SIZE, i % CHUNK_SIZE);
                        string tileName = World.GetTileName(blocks[i - (SECTION_SIZE * 3)]);
                        SetTile(pos, tileName);
                    }
                    break;
            }
            ReceivedSections = offset;
        }
    }
}
