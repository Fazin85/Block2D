using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Block2D.Client.UI
{
    public delegate void TextSubmitted(string text);

    public class Chat
    {
        public event TextSubmitted TextSubmitted;

        public bool IsOpen { get; private set; }

        private string _currentText;

        public Chat(GameWindow window)
        {
            window.TextInput += OnTextInput;
            _currentText = string.Empty;
        }

        public void Draw(SpriteBatch spriteBatch, ClientAssetManager assetManager, Vector2 position, RectangleF cameraBounds)
        {
            if (!IsOpen)
            {
                return;
            }

            RectangleF chatBackgroundRectangle = new(position.ToPoint() + new Point(0, 8), new(cameraBounds.Width, 18));

            spriteBatch.DrawRectangle(chatBackgroundRectangle, Color.White);
            spriteBatch.DrawString(assetManager.Font, _currentText, position, Color.White);
        }

        private void OnTextInput(object sender, TextInputEventArgs args)
        {
            bool shouldAddKey = true;

            if (args.Key == Keys.T && !IsOpen)
            {
                IsOpen = true;
                shouldAddKey = false;
            }

            if (!IsOpen || args.Key == Keys.LeftShift || args.Key == Keys.RightShift)
            {
                return;
            }

            if ((IsValidKey(args.Key) || args.Character == '/') && shouldAddKey)
            {
                string textToAdd = string.Empty;

                if (args.Character == '/')
                {
                    textToAdd = args.Character.ToString();
                }
                else if (args.Key == Keys.Space)
                {
                    textToAdd = " ";
                }
                else if (args.Key.IsNumber(out int number))
                {
                    textToAdd = number.ToString();
                }
                else
                {
                    textToAdd = args.Key.ToString();
                }

                _currentText += textToAdd;
            }

            if (args.Key == Keys.Back)
            {
                if (!string.IsNullOrEmpty(_currentText))
                {
                    _currentText = _currentText[..^1];
                }
            }

            if (args.Key == Keys.Enter && IsOpen)
            {
                IsOpen = false;
                TextSubmitted.Invoke(_currentText);
                _currentText = string.Empty;
            }
        }

        private bool IsValidKey(Keys key)
        {
            return key == Keys.A || key == Keys.B || key == Keys.C || key == Keys.D || key == Keys.E || key == Keys.F || key == Keys.G || key == Keys.H ||
                key == Keys.I || key == Keys.J || key == Keys.K || key == Keys.L || key == Keys.M || key == Keys.N || key == Keys.O || key == Keys.P || key == Keys.Q ||
                key == Keys.R || key == Keys.S || key == Keys.T || key == Keys.U || key == Keys.V || key == Keys.W || key == Keys.X || key == Keys.Y || key == Keys.Z || key == Keys.Space || key.IsNumber();
        }
    }
}
