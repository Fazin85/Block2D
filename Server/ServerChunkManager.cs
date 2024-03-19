using Block2D.Common;
using Block2D.Server.WorldGen;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

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

        public ServerChunkManager(string dimensionName, int seed)
        {
            Chunks = new();
            _generator = new(dimensionName, seed);
        }

        public bool GetOrTryAddChunk(Point position, string dimensionId, out ServerChunk chunk)
        {
            if (Chunks.ContainsKey(position))
            {
                chunk = Chunks[position];
                return true;
            }
            else
            {
                _generator.GenerateChunk(
                    position,
                    dimensionId,
                    Main.InternalServer.World,
                    out chunk
                );
                return Chunks.TryAdd(chunk.Position, chunk);
            }
        }
    }
}
