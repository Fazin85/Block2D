using Block2D.Common;
using Block2D.Modding.DataStructures;
using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace Block2D.Modding.ContentLoaders
{
    public class TileLoader : ContentLoader
    {
        public TileLoader(Mod mod)
        {
            Mod = mod;
            FilesPath = Main.ModsDirectory + "/" + Mod.InternalName + "/TileData";
        }

        public bool TryLoadTiles(out ModTile[] tiles)
        {
            tiles = null;
            if (!ModContains())
            {
                return false;
            }

            string[] tileFilePaths = GetFilePaths();
            List<ModTile> modTileList = new();

            for (int i = 0; i < tileFilePaths.Length; i++)
            {
                if (!tileFilePaths[i].Contains(LUA_FILE_EXTENSION))
                {
                    return false;
                }

                ModTile tile = new();

                Mod.Script.DoFile(tileFilePaths[i]);

                DynValue nameVal = Mod.Script.Call(Mod.Script.Globals["GetName"]);
                string name = nameVal.String;

                tile.Name = name;

                DynValue texVal = Mod.Script.Call(Mod.Script.Globals["GetTextureName"]);
                string textureName = texVal.String;

                tile.TextureName = textureName;

                if (Mod.Script.Globals["GetSoundEffectName"] != null)
                {
                    DynValue hitSoundEffectVal = Mod.Script.Call(
                        Mod.Script.Globals["GetSoundEffectName"]
                    );

                    string hitSoundEffectName = hitSoundEffectVal.String;

                    tile.HitSoundEffectName = hitSoundEffectName;
                }

                DynValue scaleVal = Mod.Script.Call(Mod.Script.Globals["GetScale"]);
                float scale = (float)scaleVal.Number;

                tile.TextureScale = scale;

                tile.DrawColor = GetTileColor();

                modTileList.Add(tile);
            }

            tiles = modTileList.ToArray();

            return true;
        }

        private Color GetTileColor()
        {
            DynValue DrawColorR = Mod.Script.Call(Mod.Script.Globals["GetDrawColorR"]);
            DynValue DrawColorG = Mod.Script.Call(Mod.Script.Globals["GetDrawColorG"]);
            DynValue DrawColorB = Mod.Script.Call(Mod.Script.Globals["GetDrawColorB"]);
            DynValue DrawColorAlpha = Mod.Script.Call(Mod.Script.Globals["GetDrawColorAlpha"]);

            int r = (int)DrawColorR.Number;
            int g = (int)DrawColorG.Number;
            int b = (int)DrawColorB.Number;
            int alpha = (int)DrawColorAlpha.Number;

            return new(r, g, b, alpha);
        }
    }
}
