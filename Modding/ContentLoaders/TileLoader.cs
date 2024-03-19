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
            : base(mod)
        {
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

                Script script = new();
                Main.SetupScript(script);

                ModTile tile = new();

                script.DoFile(tileFilePaths[i]);

                DynValue nameVal = script.Call(script.Globals["GetName"]);
                string name = nameVal.String;

                tile.Name = name;

                if (script.Globals["GetTextureName"] != null)
                {
                    DynValue texVal = script.Call(script.Globals["GetTextureName"]);
                    string textureName = texVal.String;

                    tile.TextureName = textureName;
                }

                if (script.Globals["GetSoundEffectName"] != null)
                {
                    DynValue hitSoundEffectVal = script.Call(script.Globals["GetSoundEffectName"]);

                    string hitSoundEffectName = hitSoundEffectVal.String;

                    tile.HitSoundEffectName = hitSoundEffectName;
                }

                DynValue scaleVal = script.Call(script.Globals["GetScale"]);
                float scale = (float)scaleVal.Number;

                tile.TextureScale = scale;

                DynValue tickableVal = script.Call(script.Globals["Tickable"]);
                bool tickable = tickableVal.Boolean;

                tile.Tickable = tickable;

                tile.DrawColor = GetTileColor(script);

                tile.Script = script;

                modTileList.Add(tile);
            }

            tiles = modTileList.ToArray();

            return true;
        }

        private Color GetTileColor(Script script)
        {
            DynValue DrawColorR = script.Call(script.Globals["GetDrawColorR"]);
            DynValue DrawColorG = script.Call(script.Globals["GetDrawColorG"]);
            DynValue DrawColorB = script.Call(script.Globals["GetDrawColorB"]);
            DynValue DrawColorAlpha = script.Call(script.Globals["GetDrawColorAlpha"]);

            int r = (int)DrawColorR.Number;
            int g = (int)DrawColorG.Number;
            int b = (int)DrawColorB.Number;
            int alpha = (int)DrawColorAlpha.Number;

            return new(r, g, b, alpha);
        }
    }
}
