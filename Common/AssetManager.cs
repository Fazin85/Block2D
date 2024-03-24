using Block2D.Modding;
using Block2D.Modding.DataStructures;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Block2D.Common
{
    public class AssetManager : ModLoader
    {
        private readonly ContentManager _contentManager;
        private Texture2D _playerTexture;
        private readonly Dictionary<string, Texture2D> _textures;
        public SpriteFont Font { get; private set; }

        public AssetManager(ContentManager contentManager)
            : base()
        {
            _contentManager = contentManager;
            _textures = new();
        }

        public void LoadContent()
        {
            //load font
            Font = _contentManager.Load<SpriteFont>("gamefont");

            //load tiles with modloader

            LoadPlayerTextures();

            LoadAllMods();

            foreach(ModTexture tex in LoadedContent.Textures.Values)
            {
                _textures.Add(tex.Name, tex.Texture);
            }
        }

        public void LoadPlayerTextures()
        {
            _playerTexture = _contentManager.Load<Texture2D>("Player_Texture");
        }

        public Texture2D GetPlayerTexture()
        {
            return _playerTexture;
        }

        public Texture2D GetTexture(string textureName)
        {
            return _textures[textureName];
        }

        public ModTile GetTile(string tileName)
        {
            return LoadedContent.Tiles[tileName];
        }
    }
}
