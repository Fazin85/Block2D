using Block2D.Common;
using Block2D.Common.ID;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Block2D.Server.World
{
    public class ServerWorld : WorldData
    {
        public bool IsLoaded { get; }

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
        private readonly InternalServer _server;

        public ServerWorld(AssetManager assetManager, InternalServer server, string name) : base(assetManager, ProgramSide.Server)
        {
            Name = name; //must do this beforce creating directories
            CreateNeededDirectories();
            Dimensions = [];
            Players = [];
            _currentTick = 0;
            _server = server;

            CreateDimensions();
        }

        private void CreateDimensions()
        {
            ServerDimension overworld = new(_server, DimensionID.OVERWORLD, _seed, 781250, 781250, 400, 500);
            Dimensions.Add(DimensionID.OVERWORLD, overworld);
        }

        public void LoadContent()
        {
            LoadTiles();
        }

        public void Update()
        {
            _currentTick++;

            if (_currentTick % 3 == 0)
            {
                _server.MessageHandler.SendPositions();

                foreach (ServerDimension dim in Dimensions.Values)
                {
                    if (dim.ChunkManager.Chunks.Count > 0)
                    {
                        foreach (
                        var chunk in dim.ChunkManager.Chunks.Values.Where(c =>
                            c.LoadAmount != ChunkLoadAmount.Unloaded
                        )
                    )
                        {
                            chunk.Tick();
                        }
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
            _server.Logger.LogInfo(player.Name + " Joined The Game");
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
                _server.Logger.LogWarning(e.Message);
            }
            return false;
        }

        public ushort GetTileID(string name)
        {
            if (!LoadedTiles.ContainsKey(name))
            {
                _server.Logger.LogWarning("Tried To Get Tile That Doesn't Exist.");
            }

            return LoadedTiles[name];
        }

        public override bool GetChunkLoaded(string dimensionID, Point chunkPosition, out ServerChunk chunk)
        {
            return Dimensions[dimensionID]
                .ChunkManager.Chunks.TryGetValue(chunkPosition, out chunk);
        }
    }
}
