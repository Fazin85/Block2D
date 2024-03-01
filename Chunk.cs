using Block2D.Server;
using Microsoft.Xna.Framework;
using System;

namespace Block2D
{
    public struct Chunk : ITickable
    {
        public const int CHUNK_SIZE = 64;

        public readonly ChunkLoadAmount LoadAmount
        {
            get => _loadAmount;
        }

        public Vector2 Position
        {
            readonly get => _position;
            set => _position = value;
        }

        public readonly Tile[,] Tiles
        {
            get => _tiles;
        }

        private readonly Tile[,] _tiles;
        private readonly ChunkLoadAmount _loadAmount;
        private Vector2 _position;

        public Chunk(Vector2 position)
        {
            _tiles = new Tile[CHUNK_SIZE, CHUNK_SIZE];
            _loadAmount = ChunkLoadAmount.Unloaded;
            _position = position;
        }

        public readonly void Tick()
        {
            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    _tiles[x, y].Tick();
                }
            }
        }

        public readonly void SetTile(Vector2 position, ushort id)
        {
            if (position.X >= CHUNK_SIZE || position.X < 0)
            {
                Console.WriteLine("Position X is out of bounds");
                return;
            }

            if (position.Y >= CHUNK_SIZE || position.Y < 0)
            {
                Console.WriteLine("Position Y is out of bounds");
                return;
            }

            _tiles[(int)position.X, (int)position.Y].Set(id);
        }

        public readonly Tile GetTile(Vector2 position)
        {
            if (position.X >= CHUNK_SIZE || position.X < 0)
            {
                Console.WriteLine("Position X is out of bounds");
                return new Tile();
            }

            if (position.Y >= CHUNK_SIZE || position.Y < 0)
            {
                Console.WriteLine("Position Y is out of bounds");
                return new Tile();
            }

            return _tiles[(int)position.X, (int)position.Y];
        }
    }
}
