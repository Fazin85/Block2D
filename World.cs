using System;
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
        private List<ServerPlayer> _players;
        private int _tickCounter;

        public void Tick()
        {
            _tickCounter++;
        }

        public void AddPlayer(ServerPlayer player)
        {
            _players.Add(player);
        }

        public bool RemovePlayer(ushort id)
        {
            try
            {
                for (int i = 0; i < _players.Count; i++)
                {
                    ServerPlayer player = _players[i];
                    if (player.ID == id)
                    {
                        _players.RemoveAt(i);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Main.Logger.Warn(e);
            }
            return false;
        }

        public bool TryAddChunk(Chunk chunk)
        {
            if (_chunks.TryAdd(chunk.Position, chunk))
            {
                _chunks[chunk.Position] = chunk;
                return true;
            }
            return false;
        }

        public bool IsChunkLoaded(Vector2 position)
        {
            return _chunks.TryGetValue(position, out Chunk chunk)
                && chunk.LoadAmount != ChunkLoadAmount.Unloaded;
        }
    }
}
