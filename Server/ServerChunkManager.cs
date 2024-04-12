using Block2D.Server.WorldGen;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Block2D.Server
{
    public class ServerChunkManager
    {
        public int ChunkCount
        {
            get => Chunks.Count;
        }
        public Dictionary<Point, ServerChunk> Chunks { get; private set; }

        private readonly TerrainGenerator _generator;
        private readonly ServerWorld _world;

        public ServerChunkManager(InternalServer server, string dimensionName, int seed)
        {
            _world = server.World;
            Chunks = [];
            _generator = new(dimensionName, seed, server);
        }

        public bool GetOrTryAddChunk(Point position, string dimensionId, out ServerChunk chunk)
        {
            if (Chunks.TryGetValue(position, out ServerChunk value))
            {
                chunk = value;
                return true;
            }
            else
            {
                _generator.GenerateChunk(
                    position,
                    dimensionId,
                    out chunk
                );
                return Chunks.TryAdd(chunk.Position, chunk);
            }
        }
    }
}
