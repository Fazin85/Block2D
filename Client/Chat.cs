using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Block2D.Client
{
    public class Chat
    {
        public bool IsOpen;

        private string _currentText;

        public Chat(GameWindow window)
        {
            window.TextInput += OnTextInput;
            _currentText = string.Empty;
        }

        public void Draw(SpriteBatch spriteBatch, ClientAssetManager assetManager, Vector2 position)
        {
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

            if (IsValidKey(args.Key) && shouldAddKey)
            {
                if (args.Key == Keys.Space)
                {
                    _currentText += " ";
                }
                else if (args.Key.IsNumber(out int number))
                {
                    _currentText += number.ToString();
                }
                else
                {
                    _currentText += args.Key.ToString();
                }
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
                _currentText = string.Empty;
            }
        }

        private bool IsValidKey(Keys key)
        {
            return key == Keys.A || key == Keys.B || key == Keys.C || key == Keys.D || key == Keys.E || key == Keys.F || key == Keys.G || key == Keys.H ||
                key == Keys.I || key == Keys.J || key == Keys.K || key == Keys.L || key == Keys.M || key == Keys.N || key == Keys.O || key == Keys.P || key == Keys.Q ||
                key == Keys.R || key == Keys.S || key == Keys.T || key == Keys.U || key == Keys.V || key == Keys.W || key == Keys.X || key == Keys.Y || key == Keys.Z || key == Keys.Space || key.IsNumber(out _);
        }
    }
}
