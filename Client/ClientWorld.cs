using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Block2D.Client
{
    public class ClientWorld : ITickable
    {
        public Chunk[] Chunks
        {
            get => _chunks.Values.ToArray();
        }

        private readonly Dictionary<Vector2, Chunk> _chunks;
        private readonly List<ClientPlayer> _players;
        private Client _localPlayerClient;

        public ClientWorld(Client localClient)
        {
            _chunks = new();
            _players = new();
            _localPlayerClient = localClient;
        }

        public void Tick() { }

        public void AddPlayer(ClientPlayer player)
        {
            _players.Add(player);
        }

        public bool RemovePlayer(ushort id)
        {
            if (id == _localPlayerClient.ID)
            {
                Main.Logger.Warn("Tried To Remove Local Player");
                return false;
            }

            try
            {
                for (int i = 0; i < _players.Count; i++)
                {
                    ClientPlayer player = _players[i];
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
            return _chunks.ContainsKey(position);
        }
    }
}
