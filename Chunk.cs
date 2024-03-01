using System;
using Block2D.Server;
using Microsoft.Xna.Framework;

namespace Block2D
{
    public class Chunk : ITickable
    {
        public const int CHUNK_SIZE = 64;

        public ChunkLoadAmount LoadAmount
        {
            get => _loadAmount;
        }

        public Vector2 Position
        {
            get => _position;
        }

        private Tile[,] _tiles;
        private ChunkLoadAmount _loadAmount;
        private Vector2 _position;

        public Chunk()
        {
            _tiles = new Tile[CHUNK_SIZE, CHUNK_SIZE];
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

        public void SetTile(Vector2 position, ushort id)
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

        public void SetTile(Vector2 position, int microTilePosition, ushort microTileId)
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

            _tiles[(int)position.X, (int)position.Y].Set(microTilePosition, microTileId);
        }

        public Tile GetTile(Vector2 position)
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
