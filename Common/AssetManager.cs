using Block2D.Modding;
using Block2D.Modding.DataStructures;
using Microsoft.Xna.Framework;
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

        public void LoadAllContent()
        {
            //load font
            Font = _contentManager.Load<SpriteFont>("pixeled");

            //load tiles with modloader

            LoadPlayerTextures();

            LoadAllMods();

            ModContent loadedContent = GetLoadedContent();

            for (int i = 0; i < loadedContent.Textures.Values.Count; i++)
            {
                ModTexture tex = loadedContent.Textures.Values.ElementAt(i);
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

        public Color GetTileDrawColor(string tileName)
        {
            ModContent content = GetLoadedContent();
            return content.Tiles[tileName].DrawColor;
        }
    }
}
