using Block2D.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Block2D.Client
{
    public class ClientWorld : ITickable
    {
        public List<ClientPlayer> Players { get; private set; }

        public Dictionary<Point, Chunk> Chunks { get; private set; }

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
            return Chunks.TryAdd(chunk.Position, chunk);
        }

        public bool IsChunkLoaded(Point position)
        {
            return Chunks.ContainsKey(position);
        }

        public bool GetChunkLoaded(Point chunkPosition, out Chunk chunk)
        {
            return Chunks.TryGetValue(chunkPosition, out chunk);
        }

        public object TrySetTile(Vector2 position, out Tile tile)
        {
            tile = new Tile();
            return new NotImplementedException();
        }

        public bool TryGetTile(Point worldPosition, out Tile tile)
        {
            Point chunkPosition = worldPosition.ToChunkCoords();
            if (GetChunkLoaded(chunkPosition, out Chunk chunk))
            {
                int x = Chunk.CHUNK_SIZE - Math.Abs(chunkPosition.X - worldPosition.X);
                int y = Chunk.CHUNK_SIZE - Math.Abs(chunkPosition.Y - worldPosition.Y);

                tile = chunk.GetTile(new(x, y));
                return true;
            }
            tile = new();
            return false;
        }
    }
}
