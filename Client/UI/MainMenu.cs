using Block2D.Common;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using Myra.Graphics2D.UI;

namespace Block2D.Client.UI
{
    public class MainMenu
    {
        private readonly Button _quitButton;
        private readonly Button _singlePlayerButton;
        private readonly Grid _grid;
        private readonly Desktop _desktop;

        public MainMenu(Main main)
        {
            MyraEnvironment.Game = main;

            _grid = new Grid()
            {
                RowSpacing = 8,
                ColumnSpacing = 8,
            };

            _grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            _quitButton = new Button()
            {
                Width = 128,
                Height = 32,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,

                Content = new Label()
                {
                    Text = "Quit",
                }
            };

            _quitButton.Click += (s, e) =>
            {
                Main.ShouldExit = true;
            };

            _grid.Widgets.Add(_quitButton);

            _singlePlayerButton = new Button()
            {
                Width = 128,
                Height = 32,
                Top = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,

                Content = new Label()
                {
                    Text = "Single Player",
                }
            };

            _singlePlayerButton.Click += (s, e) =>
            {
                main.StartSinglePlayer();
            };

            _grid.Widgets.Add(_singlePlayerButton);

            _desktop = new()
            {
                Root = _grid
            };
        }

        public void Draw(SpriteBatch spriteBatch, ClientAssetManager assetManager)
        {
            _desktop.Render();
        }

        public void Update()
        {
        }
    }
}
