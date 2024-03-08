using Block2D.Server.WorldGen;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Block2D.Server
{
    public class ServerChunkManager
    {
        private readonly Dictionary<Point, ServerChunk> _chunks;

        private readonly TerrainGenerator _generator;

        public ServerChunkManager(string dimensionName, int seed)
        {
            _chunks = new();
            _generator = new(dimensionName, seed);
        }

        public bool GetOrTryAddChunk(Point position, out ServerChunk chunk)
        {
            if (_chunks.ContainsKey(position))
            {
                chunk = _chunks[position];
                return true;
            }
            else
            {
                _generator.GenerateChunk(position, out chunk);
                return TryAddChunk(chunk);
            }
        }

        private bool TryAddChunk(ServerChunk chunk)
        {
            return _chunks.TryAdd(chunk.Position, chunk);
        }
        /// <summary>Checks if the chunk dictionary contains the key <see cref="Vector2"/>, and that the chunk tied to the <see cref="Vector2"/> position isn't unloaded</summary>
        public bool IsChunkLoaded(Point position)
        {
            return _chunks.TryGetValue(position, out ServerChunk chunk)
                && chunk.LoadAmount != ChunkLoadAmount.Unloaded;
        }
    }
}
