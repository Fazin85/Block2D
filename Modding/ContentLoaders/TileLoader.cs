﻿using Block2D.Common;
using Newtonsoft.Json;
using System.IO;

namespace Block2D.Modding.ContentLoaders
{
    public class TileLoader
    {
        private readonly Mod _mod;
        private readonly string _tileFilesPath;

        public TileLoader(Mod mod)
        {
            _mod = mod;
            _tileFilesPath = Main.ModsDirectory + _mod.Name + "Tiles";
        }

        private bool DoesModHaveTiles()
        {
            if (Directory.Exists(_tileFilesPath))
            {
                string[] tileFiles = Directory.GetFiles(_tileFilesPath);
                return tileFiles.Length > 0;
            }
            return false;
        }

        private string[] GetTileFilePaths()
        {
            string[] files = Directory.GetFiles(_tileFilesPath);
            if (files.Length == 0)
            {
                Main.Logger.Warn("There Are No Files To Get!");
                return null;
            }
            return files;
        }

        public ModTile[] LoadTiles()
        {
            if (!DoesModHaveTiles())
            {
                Main.Logger.Fatal("Tried To Load Tiles From A Mod That Doesn't Have Any!");
                return null;
            }

            string[] tileFileInfo = GetTileFilePaths();
            ModTile[] tiles = new ModTile[tileFileInfo.Length];

            for (int i = 0; i < tileFileInfo.Length; i++)
            {
                tiles[i] = (ModTile)JsonConvert.DeserializeObject(tileFileInfo[i]);
            }
            return tiles;
        }
    }
}
