using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Block2D.Client
{
    public class ClientPlayer : ITickable
    {
        public bool IsLocal
        {
            get => _id == Main.Client.ID;
        }

        public ushort ID
        {
            get => _id;
        }

        private Vector2 _position;
        private Vector2 _velocity;
        private readonly int _health;
        private readonly ushort _id;

        public ClientPlayer(ushort id)
        {
            _id = id;
        }

        public void Tick()
        {
            if (IsLocal)
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
}
