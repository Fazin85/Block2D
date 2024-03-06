using Block2D.Common;
using Newtonsoft.Json;

namespace Block2D.Modding.ContentLoaders
{
    public class TileLoader : ContentLoader
    {
        public TileLoader(Mod mod)
        {
            Mod = mod;
            FilesPath = Main.ModsDirectory + Mod.DisplayName + "Tiles";
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
