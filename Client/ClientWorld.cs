using Block2D.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Block2D.Client
{
    public class ClientWorld : ITickable
    {
        public List<ClientPlayer> Players { get; private set; }

        public Dictionary<Vector2, Chunk> Chunks { get; private set; }

        public ClientWorld()
        {
            Chunks = new();
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
            if (Chunks.TryAdd(chunk.Position, chunk))
            {
                Chunks[chunk.Position] = chunk;
                return true;
            }
            return false;
        }

        public bool IsChunkLoaded(Vector2 position)
        {
            return Chunks.ContainsKey(position);
        }

        public bool GetChunkLoaded(Vector2 position, out Chunk chunk)
        {
            chunk = new(position);
            return Chunks.TryGetValue(chunk.Position, out chunk);
        }
    }
}
