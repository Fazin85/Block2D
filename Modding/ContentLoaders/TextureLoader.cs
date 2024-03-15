using Block2D.Common;
using Block2D.Modding.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Block2D.Modding.ContentLoaders
{
    public class TextureLoader : ContentLoader
    {
        public TextureLoader(Mod mod)
        {
            Mod = mod;
            FilesPath = Main.ModsDirectory + "/" + Mod.InternalName + "/Textures";
        }

        public bool TryLoadTextures(out ModTexture[] textures)
        {
            textures = null;
            if (!ModContains())
            {
                return false;
            }

            GraphicsDevice graphicsDevice = Main.GetGraphicsDevice();

            string[] textureFilePaths = GetFilePaths();
            textures = new ModTexture[textureFilePaths.Length];

            for (int i = 0; i < textureFilePaths.Length; i++)
            {
                if (!textureFilePaths[i].Contains(".txt"))
                {
                    return false;
                }

                StreamReader sr = new(textureFilePaths[i]);
                string name = sr.ReadLine();
                textures[i].Name = name;
                string texturePath = sr.ReadLine();
                textures[i].Texture = Texture2D.FromFile(graphicsDevice, FilesPath + "/" + texturePath);
                sr.Dispose();
            }

            return true;
        }
    }
}
