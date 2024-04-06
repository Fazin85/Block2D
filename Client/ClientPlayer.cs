using Block2D.Common;
using Block2D.Common.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Block2D.Client
{
    public class ClientPlayer
    {
        private bool IsLocal
        {
            get => _id == Main.Client.LocalPlayer.ID;
        }

        public ushort ID
        {
            get => _id;
        }

        public string Dimension
        {
            get => _dimension;
        }

        public Rectangle Hitbox
        {
            get => _hitbox;
        }

        public Vector2 Position { get; set; }
        public Vector2 PreviousPosition { get; private set; }
        public Vector2 Velocity
        {
            get => _velocity;
        }

        public string Name { get; private set; }

        private Vector2 _velocity;
        private Rectangle _hitbox;
        private readonly int _health;
        private readonly ushort _id;
        private readonly string _dimension = DimensionID.OVERWORLD;

        public ClientPlayer(ushort id, string name)
        {
            _id = id;
            _dimension = DimensionID.OVERWORLD;
            _hitbox = new(Position.ToPoint(), new(16, 16));
            Name = name;
        }

        public void Tick(GameTime gameTime)
        {
            if (IsLocal)
            {
                _velocity = Vector2.Zero;

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
                PreviousPosition = Position;
                Position += _velocity * (gameTime.ElapsedGameTime.Milliseconds / 16);
                _hitbox.Location = Position.ToPoint();

                HandleCollision();
            }
        }

        private void HandleCollision()
        {
            for (int x = _hitbox.Left; x <= _hitbox.Right; x++)
            {
                for (int y = _hitbox.Top; y <= _hitbox.Bottom; y++)
                {
                    Point currentPosition = new(x >> 4, y >> 4);
                    if (Main.Client.World.TryGetTile(currentPosition, out ClientTile tile))
                    {
                        if (tile.Collidable)
                        {
                            Rectangle currentTileRect =
                                new(new(currentPosition.X * CC.TILE_SIZE, currentPosition.Y * CC.TILE_SIZE), new(CC.TILE_SIZE, CC.TILE_SIZE));
                            if (_hitbox.Intersects(currentTileRect))
                            {
                                Vector2 depth = Helper.GetIntersectionDepth(
                                    _hitbox,
                                    currentTileRect
                                );

                                if (depth != Vector2.Zero)
                                {
                                    if (Math.Abs(depth.X) < Math.Abs(depth.Y))
                                    {
                                        Position = new(Position.X + depth.X, Position.Y);
                                    }
                                    else
                                    {
                                        Position = new(Position.X, Position.Y + depth.Y);
                                    }

                                    _hitbox.Location = Position.ToPoint();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
