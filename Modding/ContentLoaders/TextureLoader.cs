using Block2D.Common;
using Block2D.Modding.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace Block2D.Modding.ContentLoaders
{
    public class TextureLoader : ContentLoader
    {
        public TextureLoader(Mod mod) : base(mod)
        {
            FilesPath = Main.ModsDirectory + "/" + Mod.InternalName + "/TextureData";
        }

        public bool TryLoadTextures(out ModTexture[] textures)
        {
            textures = null;
            if (!ModContains())
            {
                return false;
            }

            GraphicsDevice graphicsDevice = Main.GetGraphicsDevice();
            List<ModTexture> textureList = new();

            string[] textureFilePaths = GetFilePaths();

            for (int i = 0; i < textureFilePaths.Length; i++)
            {
                if (!textureFilePaths[i].Contains(LUA_FILE_EXTENSION))
                {
                    return false;
                }

                Script script = new();
                Main.SetupScript(script);

                ModTexture texture = new();

                script.DoFile(textureFilePaths[i]);
                DynValue nameVal = script.Call(script.Globals["GetName"]);
                string name = nameVal.String;
                texture.Name = name;

                DynValue pathVal = script.Call(script.Globals["GetPath"]);
                string texturePath = pathVal.String;
                texture.Texture = Texture2D.FromFile(
                    graphicsDevice,
                    FilesPath + "/Textures/" + texturePath
                );

                textureList.Add(texture);
            }

            textures = textureList.ToArray();

            return true;
        }
    }
}
