using System.IO;
using Block2D.Common;

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
                StreamReader sr = new(tileFileInfo[i]);
                string name = sr.ReadLine();
                tiles[i].Name = name;
                string texturePath = sr.ReadLine();
                tiles[i].TexturePath = texturePath;
                string hitSoundEffectName = sr.ReadLine();
                tiles[i].HitSoundEffectName = hitSoundEffectName;

                if (float.TryParse(sr.ReadLine(), out float textureScale))
                {
                    tiles[i].TextureScale = textureScale;
                }
                else
                {
                    Main.Logger.Fatal("Tried To Load Tile With Invalid Scale.\nBroken Tile Path: " + tileFileInfo[i]);
                    Main.ForceQuitModloader();
                }
            }
            return true;
        }
    }
}
