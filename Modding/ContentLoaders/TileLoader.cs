using System.IO;
using Block2D.Common;
using Block2D.Modding.DataStructures;

namespace Block2D.Modding.ContentLoaders
{
    public class TileLoader : ContentLoader
    {
        public TileLoader(Mod mod)
        {
            Mod = mod;
            FilesPath = Main.ModsDirectory + "/" + Mod.InternalName + "/Tiles";
        }

        public bool TryLoadTiles(out ModTile[] tiles)
        {
            tiles = null;
            if (!ModContains())
            {
                return false;
            }

            string[] tileFilePaths = GetFilePaths();
            tiles = new ModTile[tileFilePaths.Length];

            for (int i = 0; i < tileFilePaths.Length; i++)
            {
                if (!tileFilePaths[i].Contains(".txt"))
                {
                    return false;
                }

                StreamReader sr = new(tileFilePaths[i]);
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
                    Main.Logger.Fatal("Tried To Load Tile With Invalid Scale.\nBroken Tile Path: " + tileFilePaths[i]);
                    Main.ForceQuitModloader();
                }
            }
            return true;
        }
    }
}
