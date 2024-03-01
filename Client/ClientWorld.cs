using Block2D.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Block2D.Client
{
    public class ClientWorld : ITickable
    {
        public Chunk[] Chunks
        {
            get => _chunks.Values.ToArray();
        }

        public List<ClientPlayer> Players { get; private set; }

        private readonly Dictionary<Vector2, Chunk> _chunks;

        public ClientWorld()
        {
            _chunks = new();
            Players = new();
        }

        public void Tick() { }

        public void AddPlayer(ClientPlayer player)
        {
            Players.Add(player);
        }

        public bool RemovePlayer(ushort id)
        {
            if (id == Main.Client.LocalPlayer.ID)
            {
                Main.Logger.Warn("Tried To Remove Local Player");
                return false;
            }

            try
            {
                for (int i = 0; i < Players.Count; i++)
                {
                    ClientPlayer player = Players[i];
                    if (player.ID == id)
                    {
                        Players.RemoveAt(i);
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

        public ClientPlayer GetPlayerFromId(ushort id)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                ClientPlayer player = Players[i];
                if (player.ID == id)
                {
                    return Players[i];
                }
            }
            Main.Logger.Warn("Could Not Find Player With ID: " + id);
            return null;
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
