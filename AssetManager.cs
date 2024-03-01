using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Block2D
{
    public class AssetManager
    {
        private readonly ContentManager _contentManager;
        private readonly Dictionary<string, Texture2D> _blockTextures;

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
        }

        public Texture2D GetBlockTexture(string blockId)
        {
            return _blockTextures[blockId];
        }
    }
}
