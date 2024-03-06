using Block2D.Common;
using Block2D.Common.ID;
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

        public Dictionary<string, ServerDimension> Dimensions { get; private set; }

        private readonly List<ServerPlayer> _players;
        private int _tickCounter;
        private readonly int _seed;

        public ServerWorld(string name)
        {
            Name = name;//must do this beforce creating directories
            CreateNeededDirectories();
            Dimensions = new();
            _players = new();
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
        }

        public void AddPlayer(ServerPlayer player)
        {
            _players.Add(player);
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
    }
}
