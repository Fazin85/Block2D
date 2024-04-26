using Block2D.Client;
using Block2D.Modding.DataStructures;
using Microsoft.Xna.Framework;

namespace Block2D.Common
{
    public class ClientChunk
    {
        private const int SECTION_SIZE = CC.CHUNK_SIZE * CC.CHUNK_SIZE / CC.CHUNK_SECTION_COUNT;
        public byte ReceivedSections { get; private set; }
        public Point Position { get; private set; }
        private readonly ClientTile[,] _tiles;
        private readonly Client.Client _client;

        public ClientChunk(Point position, Client.Client client)
        {
            Position = position;
            ReceivedSections = 0;
            _client = client;
            _tiles = new ClientTile[CC.CHUNK_SIZE, CC.CHUNK_SIZE];
        }

        public void SetTile(Point position, string tileName)
        {
            if (!_client.CurrentWorld.LoadedTiles.TryGetValue(tileName, out ushort id))
            {
                return;
            }

            if (position.X >= CC.CHUNK_SIZE || position.X < 0)
            {
                _client.LogFatal("Position X is out of bounds");
                return;
            }

            if (position.Y >= CC.CHUNK_SIZE || position.Y < 0)
            {
                _client.LogFatal("Position Y is out of bounds");
                return;
            }

            ModTile tile = _client.AssetManager.GetTile(tileName);

            _tiles[position.X, position.Y].Set(id, false, tile.Collidable);
        }

        public ClientTile GetTile(Point position) //we have to return a new tile in the error checking parts because the renderer is funny like that
        {
            if (position.X >= CC.CHUNK_SIZE || position.X < 0)
            {
                _client.LogFatal("Position X is out of bounds");
                return new();
            }

            if (position.Y >= CC.CHUNK_SIZE || position.Y < 0)
            {
                _client.LogFatal("Position Y is out of bounds");
                return new();
            }

            return _tiles[position.X, position.Y];
        }

        public void SetSection(ushort[] blocks, byte offset)
        {
            switch (offset)
            {
                case 0:
                    for (int i = 0; i < SECTION_SIZE; i++)
                    {
                        Point pos = new(i / CC.CHUNK_SIZE, i % CC.CHUNK_SIZE);

                        string tileName = _client.CurrentWorld.GetTileName(blocks[i]);
                        SetTile(pos, tileName);
                    }
                    break;
                case 1:
                    for (int i = SECTION_SIZE; i < SECTION_SIZE * 2; i++)
                    {
                        Point pos = new(i / CC.CHUNK_SIZE, i % CC.CHUNK_SIZE);
                        string tileName = _client.CurrentWorld.GetTileName(blocks[i - SECTION_SIZE]);
                        SetTile(pos, tileName);
                    }
                    break;
                case 2:
                    for (int i = SECTION_SIZE * 2; i < SECTION_SIZE * 3; i++)
                    {
                        Point pos = new(i / CC.CHUNK_SIZE, i % CC.CHUNK_SIZE);
                        string tileName = _client.CurrentWorld.GetTileName(blocks[i - (SECTION_SIZE * 2)]);
                        SetTile(pos, tileName);
                    }
                    break;
                case 3:
                    for (int i = SECTION_SIZE * 3; i < SECTION_SIZE * 4; i++)
                    {
                        Point pos = new(i / CC.CHUNK_SIZE, i % CC.CHUNK_SIZE);
                        string tileName = _client.CurrentWorld.GetTileName(blocks[i - (SECTION_SIZE * 3)]);
                        SetTile(pos, tileName);
                    }
                    break;
            }
            ReceivedSections = offset;
        }
    }
}
