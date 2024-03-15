using Block2D.Common;
using Block2D.Modding.DataStructures;
using Microsoft.Xna.Framework;
using System.IO;

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
                string textureName = sr.ReadLine();
                tiles[i].TextureName = textureName;
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
                    sr.Dispose();
                    return false;
                }

                if (int.TryParse(sr.ReadLine(), out var r) && int.TryParse(sr.ReadLine(), out var g) && int.TryParse(sr.ReadLine(), out var b) && int.TryParse(sr.ReadLine(), out var alpha))
                {
                    Color color = new(r, g, b, alpha);
                    tiles[i].DrawColor = color;
                }
                else
                {
                    Main.Logger.Fatal("Tried To Load Tile With Invalid Draw Color.\nBroken Tile Path: " + tileFilePaths[i]);
                    Main.ForceQuitModloader();
                    sr.Dispose();
                    return false;
                }
                sr.Dispose();
            }
            return true;
        }
    }
}
