using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Block2D.Common
{
    public class AssetManager
    {
        private readonly ContentManager _contentManager;
        private readonly Dictionary<string, Texture2D> _blockTextures;
        private Texture2D _playerTexture;

        public AssetManager(ContentManager contentManager)
        {
            _contentManager = contentManager;
            _blockTextures = new Dictionary<string, Texture2D>();
        }

        public void LoadBlockTextures()
        {
            Texture2D stone = _contentManager.Load<Texture2D>("Stone_Block");
            _blockTextures.Add("Stone_Block", stone);
            Texture2D grass = _contentManager.Load<Texture2D>("Grass_Block");
            _blockTextures.Add("Grass_Block", grass);
            Texture2D dirt = _contentManager.Load<Texture2D>("Dirt_Block");
            _blockTextures.Add("Dirt_Block", dirt);
            //Texture2D water = _contentManager.Load<Texture2D>("Water_Block");
            //_blockTextures.Add("Water_Block", water);
            Texture2D sand = _contentManager.Load<Texture2D>("Sand_Block");
            _blockTextures.Add("Sand_Block", sand);

            LoadPlayerTextures();
        }

        public void LoadPlayerTextures()
        {
            _playerTexture = _contentManager.Load<Texture2D>("Player_Texture");
        }

        public Texture2D GetPlayerTexture()
        {
            return _playerTexture;
        }

        public Texture2D GetBlockTexture(string blockId)
        {
            return _blockTextures[blockId];
        }
    }
}
