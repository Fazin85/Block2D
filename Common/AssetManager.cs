using Block2D.Modding;
using Block2D.Modding.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Block2D.Common
{
    public abstract class AssetManager : ModLoader
    {
        protected Dictionary<string, Texture2D> Textures { get; private set; }
        private readonly ProgramSide _side;

        public AssetManager(ProgramSide side)
            : base()
        {
            Textures = [];
            _side = side;
        }

        public virtual void LoadContent()
        {
            LoadAllMods();

            if (_side == ProgramSide.Client)
            {
                foreach (ModTexture tex in LoadedContent.Textures.Values)
                {
                    Textures.Add(tex.Name, tex.Texture);
                }
            }
        }

        public ModTile GetTile(string tileName)
        {
            return LoadedContent.Tiles[tileName];
        }
    }
}
