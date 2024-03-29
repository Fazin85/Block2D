﻿using System;
using System.Collections.Generic;
using Block2D.Client.Networking;
using Block2D.Common;
using Microsoft.Xna.Framework;

namespace Block2D.Client
{
    public class ClientWorld
    {
        public Dictionary<ushort, ClientPlayer> Players { get; private set; }

        public Dictionary<Point, ClientChunk> Chunks { get; private set; }

        private long _tickCounter;

        public ClientWorld()
        {
            Chunks = new();
            Players = new();
        }

        public void Tick(GameTime gameTime)
        {
            foreach (ClientPlayer currentPlayer in Players.Values)
            {
                currentPlayer.Tick(gameTime);

                if (_tickCounter % 3 == 0 && currentPlayer.ID == Main.Client.ID) //must be local player
                {
                    ClientMessageHandler.SendPosition(currentPlayer.Position);
                }
            }

            _tickCounter++;
        }

        public void AddPlayer(ClientPlayer player)
        {
            if (Players.ContainsKey(player.ID))
            {
                return;
            }

            Players.Add(player.ID, player);
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
                Players.Remove(id);
                return true;
            }
            catch (Exception e)
            {
                Main.Logger.Warn(e);
            }
            return false;
        }

        public ClientPlayer GetPlayerFromId(ushort id)
        {
            if (Players.TryGetValue(id, out ClientPlayer player))
            {
                return player;
            }
            Main.Logger.Warn("Could Not Find Player With ID: " + id);
            return null;
        }

        public bool TryAddChunk(ClientChunk chunk)
        {
            return Chunks.TryAdd(chunk.Position, chunk);
        }

        public bool IsChunkLoaded(Point position)
        {
            return Chunks.ContainsKey(position);
        }

        public bool GetChunkLoaded(Point chunkPosition, out ClientChunk chunk)
        {
            return Chunks.TryGetValue(chunkPosition, out chunk);
        }

        public void TrySetTile(Vector2 position, out ClientTile tile)
        {
            tile = new ClientTile();
            throw new NotImplementedException();
        }

        public bool TryGetTile(Point worldPosition, out ClientTile tile)
        {
            Point chunkPosition = worldPosition.ToChunkCoords();
            if (GetChunkLoaded(chunkPosition, out ClientChunk chunk))
            {
                int x = Math.Abs(chunkPosition.X - worldPosition.X);
                int y = Math.Abs(chunkPosition.Y - worldPosition.Y);

                tile = chunk.GetTile(new(x, y));
                return true;
            }
            tile = new();
            return false;
        }
    }
}
