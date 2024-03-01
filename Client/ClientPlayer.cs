using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Block2D.Client
{
    public class ClientPlayer : ITickable
    {
        private Vector2 _position;
        private Vector2 _velocity;
        private int _health;

        public void Tick()
        {
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.W))
            {
                _velocity.Y = -5f;
            }

            if (keyboard.IsKeyDown(Keys.S))
            {
                _velocity.Y = 5f;
            }

            if (keyboard.IsKeyDown(Keys.W) && keyboard.IsKeyDown(Keys.S))
            {
                _velocity.Y = 0f;
            }

            if (keyboard.IsKeyDown(Keys.A))
            {
                _velocity.X = -5f;
            }

            if (keyboard.IsKeyDown(Keys.D))
            {
                _velocity.X = 5f;
            }

            if (keyboard.IsKeyDown(Keys.A) && keyboard.IsKeyDown(Keys.D))
            {
                _velocity.X = 0f;
            }
        }
    }
}
