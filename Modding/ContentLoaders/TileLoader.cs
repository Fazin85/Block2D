using Block2D.Common;
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
            _tileFilesPath = Main.ModsDirectory + _mod.DisplayName + "Tiles";
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

        public bool TryLoadTiles(out ModTile[] tiles)
        {
            tiles = null;
            if (!DoesModHaveTiles())
            {
                return false;
            }

            string[] tileFileInfo = GetTileFilePaths();
            tiles = new ModTile[tileFileInfo.Length];

            for (int i = 0; i < tileFileInfo.Length; i++)
            {
                tiles[i] = (ModTile)JsonConvert.DeserializeObject(tileFileInfo[i]);
            }
            return true;
        }
    }
}
