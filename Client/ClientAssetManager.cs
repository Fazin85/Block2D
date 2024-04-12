using Block2D.Common;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Block2D.Client
{
    public class ClientAssetManager : AssetManager
    {
        public SpriteFont Font { get; private set; }

        private Texture2D _playerTexture;
        private readonly ContentManager _contentManager;

        public ClientAssetManager(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public override void LoadContent()
        {
            Font = _contentManager.Load<SpriteFont>("gamefont");

            LoadPlayerTextures();

            base.LoadContent();
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
            return Textures[textureName];
        }
    }
}
