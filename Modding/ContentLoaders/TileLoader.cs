using Block2D.Common;
using Newtonsoft.Json;
using System.IO;

namespace Block2D.Modding.ContentLoaders
{
    public class TileLoader : IContentLoader
    {
        public Mod Mod { get; set; }
        public string FilesPath { get; set; }

        public TileLoader(Mod mod)
        {
            Mod = mod;
            FilesPath = Main.ModsDirectory + Mod.DisplayName + "Tiles";
        }

        public bool ModContains()
        {
            if (Directory.Exists(FilesPath))
            {
                string[] tileFiles = Directory.GetFiles(FilesPath);
                return tileFiles.Length > 0;
            }
            return false;
        }

        public string[] GetFilePaths()
        {
            string[] files = Directory.GetFiles(FilesPath);
            if (files.Length == 0)
            {
                Main.Logger.Warn("There Are No Files To Get!");
                return null;
            }
            return files;
        }

        public bool TryLoadTiles(out ModTile[] tiles)
        {
            tiles = null;
            if (!ModContains())
            {
                return false;
            }

            string[] tileFileInfo = GetFilePaths();
            tiles = new ModTile[tileFileInfo.Length];

            for (int i = 0; i < tileFileInfo.Length; i++)
            {
                tiles[i] = (ModTile)JsonConvert.DeserializeObject(tileFileInfo[i]);
            }
            return true;
        }
    }
}
