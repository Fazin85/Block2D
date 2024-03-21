﻿using Block2D.Common;
using Block2D.Common.ID;
using Block2D.Modding;
using Block2D.Modding.DataStructures;
using Block2D.Server.Networking;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Block2D.Server
{
    public class ServerWorld : World, ITickable
    {
        public bool IsLoaded { get; }

        public string Name { get; private set; }

        public string ChunkDataPath
        {
            get => Main.WorldsDirectory + "/" + Name + "/ChunkData";
        }

        public string PlayerDataPath
        {
            get => Main.WorldsDirectory + "/" + Name + "/PlayerData";
        }

        public Dictionary<ushort, ServerPlayer> Players { get; private set; }

        public Dictionary<string, ServerDimension> Dimensions { get; private set; }

        private long _currentTick;
        private readonly int _seed;
        private ushort _nextTileIdToLoad;

        public ServerWorld(string name)
        {
            LoadedTiles = new();
            _nextTileIdToLoad = 0;
            Name = name; //must do this beforce creating directories
            CreateNeededDirectories();
            Dimensions = new();
            Players = new();
            _currentTick = 0;

            CreateDimensions();
        }

        private void CreateDimensions()
        {
            ServerDimension overworld = new(DimensionID.OVERWORLD, _seed, 781250, 781250, 400, 500);
            Dimensions.Add(DimensionID.OVERWORLD, overworld);
        }

        public void Tick()
        {
            _currentTick++;

            if (_currentTick % 3 == 0)
            {
                ServerMessageHandler.SendPositions();

                foreach (var dim in Dimensions.Values)
                {
                    foreach (var chunk in dim.ChunkManager.Chunks.Values.Where(c => c.LoadAmount != ChunkLoadAmount.Unloaded))
                    {
                        chunk.Tick();
                    }
                }
            }
        }

        public void AddPlayer(ServerPlayer player)
        {
            if (Players.ContainsKey(player.ID))
            {
                return;
            }

            Players.Add(player.ID, player);
            Main.Logger.Info("Added Player" + player.Name);
        }

        private void CreateNeededDirectories()
        {
            if (!Directory.Exists(ChunkDataPath))
            {
                Directory.CreateDirectory(ChunkDataPath);
            }
            if (!Directory.Exists(PlayerDataPath))
            {
                Directory.CreateDirectory(PlayerDataPath);
            }
        }

        public bool RemovePlayer(ushort id)
        {
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

        public ushort GetTileID(string name)
        {
            if (!LoadedTiles.ContainsKey(name))
            {
                Main.Logger.Warn("Tried To Get Tile That Doesn't Exist.");
            }

            return LoadedTiles[name];
        }

        public bool GetChunkLoaded(string dimensionID, Point chunkPosition, out ServerChunk chunk)
        {
            return Dimensions[dimensionID]
                .ChunkManager.Chunks.TryGetValue(chunkPosition, out chunk);
        }

        public void SetTile(string dimensionId, Point worldPosition, string id)
        {
            Point chunkPosition = worldPosition.ToChunkCoords();
            if (GetChunkLoaded(dimensionId, chunkPosition, out ServerChunk chunk))
            {
                int x = CC.CHUNK_SIZE - Math.Abs(chunkPosition.X - worldPosition.X);
                int y = CC.CHUNK_SIZE - Math.Abs(chunkPosition.Y - worldPosition.Y);

                chunk.SetTile(new(x, y), id);
            }
            else
            {
                Main.Logger.Warn("Tried To Set Tile In A Chunk That Doesn't Exist.");
            }
        }

        public bool TryGetTile(string dimensionID, Point worldPosition, out ServerTile tile)
        {
            Point chunkPosition = worldPosition.ToChunkCoords();
            if (GetChunkLoaded(dimensionID, chunkPosition, out ServerChunk chunk))
            {
                int x = CC.CHUNK_SIZE - Math.Abs(chunkPosition.X - worldPosition.X);
                int y = CC.CHUNK_SIZE - Math.Abs(chunkPosition.Y - worldPosition.Y);

                tile = chunk.GetTile(new(x, y));
                return true;
            }
            tile = new();
            return false;
        }

        public override void LoadAllTiles()
        {
            for (int i = 0; i < Main.ModLoader.LoadedModCount; i++)
            {
                Mod currentMod = Main.ModLoader.LoadedMods.ElementAt(i);
                ModTile[] tiles = currentMod.ContentManager.GetModTiles();

                LoadModTiles(tiles);
            }
        }

        protected override void LoadModTiles(ModTile[] modTiles)
        {
            for (int i = 0; i < modTiles.Length; i++)
            {
                LoadedTiles.Add(modTiles[i].Name, _nextTileIdToLoad);
                _nextTileIdToLoad++;
            }
        }
    }
}
