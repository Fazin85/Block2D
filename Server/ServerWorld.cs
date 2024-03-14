using Block2D.Common;
using Block2D.Common.ID;
using Block2D.Modding;
using Block2D.Modding.DataStructures;
using Block2D.Server.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        private readonly Dictionary<string, Mod> _mods;

        public Dictionary<string, ushort> LoadedTiles;

        private int _tickCounter;
        private readonly int _seed;
        private ushort _nextTileIdToLoad;

        public ServerWorld(string name)
        {
            _mods = Main.GetLoadedMods();
            LoadedTiles = new();
            AddMods();
            _nextTileIdToLoad = 0;
            Name = name;//must do this beforce creating directories
            CreateNeededDirectories();
            Dimensions = new();
            Players = new();
            _tickCounter = 0;

            CreateDimensions();
        }

        private void CreateDimensions()
        {
            ServerDimension overworld = new(DimensionID.OVERWORLD, _seed, 781250, 781250, 400, 500);
            Dimensions.Add(DimensionID.OVERWORLD, overworld);
        }

        private void AddMods()
        {
            Mod[] mods = _mods.Values.ToArray();

            for (int i = 0; i < mods.Length; i++)
            {
                _mods.Add(mods[i].DisplayName, mods[i]);
            }
        }

        public void Tick()
        {
            _tickCounter++;

            if (_tickCounter == 3)
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

        private void LoadAllTiles()
        {
            LoadDefaultTiles();

            for (int i = 0; i < _mods.Count; i++)
            {
                Mod currentMod = _mods.Values.ElementAt(i);
                ModTile[] tiles = currentMod.ContentManager.GetModTiles();

                LoadModTiles(tiles);
            }
        }

        private void LoadDefaultTiles()
        {
            LoadedTiles.Add(BlockID.AIR, 0);
            LoadedTiles.Add(BlockID.STONE, 1);
            LoadedTiles.Add(BlockID.DIRT, 2);
            LoadedTiles.Add(BlockID.GRASS, 3);
            _nextTileIdToLoad += 4;
        }

        private void LoadModTiles(ModTile[] modTiles)
        {
            for (int i = 0; i < modTiles.Length; i++)
            {
                _nextTileIdToLoad++;
                LoadedTiles.Add(modTiles[i].Name, _nextTileIdToLoad);
            }
        }
    }
}
