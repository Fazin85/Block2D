using System.Collections.Generic;
using System.Linq;
using Block2D.Server;
using Microsoft.Xna.Framework;

namespace Block2D
{
    public class World : ITickable
    {
        public bool IsLoaded { get; }

        public Chunk[] Chunks
        {
            get => _chunks.Values.ToArray();
        }

        private Dictionary<Vector2, Chunk> _chunks;
        private int _tickCounter;

        public void Tick()
        {
            _tickCounter++;
        }

        public bool IsChunkLoaded(Vector2 position)
        {
            return _chunks.TryGetValue(position, out Chunk chunk)
                && chunk.LoadAmount != ChunkLoadAmount.Unloaded;
        }
    }
}
