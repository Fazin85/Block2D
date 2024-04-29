using Block2D.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using Myra.Graphics2D.UI;

namespace Block2D.Client.UI
{
    public class MainMenu
    {
        private readonly Button _quitButton;
        private readonly Button _singlePlayerButton;
        private readonly Panel _panel;
        private readonly Desktop _desktop;

        //+ IS DOWN

        public MainMenu(Main main)
        {
            MyraEnvironment.Game = main;

            _panel = new();

            _quitButton = new Button()
            {
                Width = 256,
                Height = 32,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Top = -32,

                Content = new Label()
                {
                    Text = "Quit",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                }
            };

            _quitButton.Click += (s, e) =>
            {
                Main.ShouldExit = true;
            };

            _panel.Widgets.Add(_quitButton);

            _singlePlayerButton = new Button()
            {
                Width = 256,
                Height = 32,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,

                Content = new Label()
                {
                    Text = "Single Player",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                }
            };

            _singlePlayerButton.Click += (s, e) =>
            {
                main.StartSinglePlayer();
            };

            _panel.Widgets.Add(_singlePlayerButton);

            _desktop = new()
            {
                Root = _panel
            };
        }

        public void Draw(SpriteBatch spriteBatch, ClientAssetManager assetManager)
        {
            spriteBatch.Begin();

            Texture2D stone = assetManager.GetTexture("Stone");

            for (int x = 0; x < Main.GraphicsDevice.PresentationParameters.BackBufferWidth + (CC.TILE_SIZE * 4); x += CC.TILE_SIZE)
            {
                for (int y = 0; y < Main.GraphicsDevice.PresentationParameters.BackBufferHeight + (CC.TILE_SIZE * 4); y += CC.TILE_SIZE)
                {
                    Vector2 position = new(x, y);

                    spriteBatch.Draw(stone, position, Color.White);
                }
            }

            spriteBatch.End();

            _desktop.Render();
        }

        public void Update()
        {
        }
    }
}
