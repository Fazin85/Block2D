using Block2D.Modding;
using Block2D.Modding.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Block2D.Common
{
    public abstract class AssetManager : ModLoader
    {
        protected Dictionary<string, Texture2D> Textures { get; private set; }

        public AssetManager()
            : base()
        {
            Textures = [];
        }

        public virtual void LoadContent()
        {
            LoadAllMods();

            foreach (ModTexture tex in LoadedContent.Textures.Values)
            {
                Textures.Add(tex.Name, tex.Texture);
            }
        }

        public ModTile GetTile(string tileName)
        {
            return LoadedContent.Tiles[tileName];
        }
    }
}
