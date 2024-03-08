using Block2D.Common;
using Block2D.Common.ID;
using Block2D.Server.Networking;
using System;
using System.Collections.Generic;
using System.IO;

namespace Block2D.Server
{
    public class ServerWorld : ITickable
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

        private int _tickCounter;
        private readonly int _seed;

        public ServerWorld(string name)
        {
            Name = name;//must do this beforce creating directories
            CreateNeededDirectories();
            Dimensions = new();
            Players = new();
            _tickCounter = 0;

            CreateDimensions();
        }

        public void CreateDimensions()
        {
            ServerDimension overworld = new(DimensionID.OVERWORLD, _seed, 781250, 781250, 400, 500);
            Dimensions.Add(DimensionID.OVERWORLD, overworld);
        }

        public void Tick()
        {
            _tickCounter++;

            if(_tickCounter == 3)
            {
                ServerMessageHandler.SendPositions();
                _tickCounter = 0;
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

        public void CreateNeededDirectories()
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
    }
}
