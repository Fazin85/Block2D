﻿using Block2D.Common;
using Block2D.Server.IO;
using Block2D.Server.WorldGen;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Block2D.Server
{
    public class ServerChunkManager
    {
        private readonly Dictionary<Point, Chunk> _chunks;

        private readonly TerrainGenerator _generator;

        public ServerChunkManager(string dimensionName, int seed)
        {
            _chunks = new();
            _generator = new(dimensionName, seed);
        }

        /// <summary>Creates a <see cref="Chunk"/>, generates the chunk's terrain, and tries to add it to chunk dictionary.</summary>
        public bool TryAddNewChunk(Point position, out Chunk addedChunk)
        {
            addedChunk = new();
            if (_chunks.ContainsKey(position))
            {
                return false;
            }

            Chunk newChunk = new(position);
            _generator.GenerateChunkTerrain(newChunk);
            addedChunk = newChunk;
            if (TryAddChunk(newChunk))
            {
                ChunkSaver.SaveChunk(newChunk);
                return true;
            }
            return false;
        }

        private bool TryAddChunk(Chunk chunk)
        {
            if (_chunks.TryAdd(chunk.Position, chunk))
            {
                _chunks[chunk.Position] = chunk;
                return true;
            }
            return false;
        }
        /// <summary>Checks if the chunk dictionary contains the key <see cref="Vector2"/>, and that the chunk tied to the <see cref="Vector2"/> position isn't unloaded</summary>
        public bool IsChunkLoaded(Point position)
        {
            return _chunks.TryGetValue(position, out Chunk chunk)
                && chunk.LoadAmount != ChunkLoadAmount.Unloaded;
        }
    }
}
