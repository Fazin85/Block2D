﻿using Microsoft.Xna.Framework;
using System;

namespace Block2D.Common
{
    public abstract class Chunk
    {
        public const int CHUNK_SIZE = 64;
        public const int CHUNK_SECTION_COUNT = 4;
        public Point Position { get; set; }
        public Tile[,] Tiles
        {
            get => _tiles;
        }
        private readonly Tile[,] _tiles = new Tile[CHUNK_SIZE, CHUNK_SIZE];
        public World World { get; private set; }

        public Chunk(World world)
        {
            World = world;
        }

        public void SetTile(Point position, string tileName)
        {
            if (!World.LoadedTiles.ContainsKey(tileName))
            {
                return;
            }

            ushort id = World.LoadedTiles[tileName];

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

            _tiles[position.X, position.Y].Set(id);
        }

        public Tile GetTile(Point position)
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

            return _tiles[position.X, position.Y];
        }
    }
}
