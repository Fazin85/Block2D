using System;
using Block2D.Client.Networking;
using Block2D.Common;
using Block2D.Common.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

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
                    Point currentPosition = new(x, y);
                    if (Main.Client.World.TryGetTile(new(x >> 4, y >> 4), out ClientTile tile))
                    {
                        if (tile.Collidable)
                        {
                            Rectangle currentTileRect =
                                new(currentPosition, new(CC.TILE_SIZE, CC.TILE_SIZE));
                            if (_hitbox.Intersects(currentTileRect))
                            {
                                Vector2 depth = Helper.GetIntersectionDepth(
                                    _hitbox,
                                    currentTileRect
                                );

                                if (depth != Vector2.Zero)
                                {
                                    if (Math.Abs(depth.X) > Math.Abs(depth.Y))
                                    {
                                        //Position = new(Position.X, Position.Y + depth.Y);
                                    }
                                    else
                                    {
                                        //Position = new(Position.X + depth.X, Position.Y);
                                    }

                                    _hitbox.Location = Position.ToPoint();
                                }

                                var side = CollisionHelperAABB.GetCollisionSide(
                                    _hitbox,
                                    currentTileRect,
                                    _velocity
                                );
                                var pos = CollisionHelperAABB.GetCorrectedLocation(
                                    _hitbox,
                                    currentTileRect,
                                    side
                                );

                                Position = pos;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>An enumeration of the possible sides at which 2D collision occurred.</summary>
        [Flags]
        public enum CollisionSide
        {
            /// <summary>No collision occurred.</summary>
            None = 0,

            /// <summary>Collision occurred at the top side.</summary>
            Top = 1,

            /// <summary>Collision occurred at the bottom side.</summary>
            Bottom = 2,

            /// <summary>Collision occurred at the left side.</summary>
            Left = 4,

            /// <summary>Collision occurred at the right side.</summary>
            Right = 8
        }

        /// <summary>A collection of helper methods for 2D collision detection and response.</summary>
        public static class CollisionHelperAABB
        {
            /// <summary>Calculates which side of a stationary object
            /// a moving object has collided with.</summary>
            /// <param name="movingObjectPreviousHitbox">The moving object's previous hitbox,
            /// from the frame prior to when collision occurred.</param>
            /// <param name="stationaryObjectHitbox">The stationary object's hitbox.</param>
            /// <param name="movingObjectVelocity">The moving object's velocity
            /// during the frame in which the collision occurred.</param>
            /// <returns>The side of the stationary object the moving object has collided with.</returns>
            public static CollisionSide GetCollisionSide(
                Rectangle movingObjectPreviousHitbox,
                Rectangle stationaryObjectHitbox,
                Vector2 movingObjectVelocity
            )
            {
                double cornerSlopeRise = 0;
                double cornerSlopeRun = 0;

                double velocitySlope = movingObjectVelocity.Y / movingObjectVelocity.X;

                //Stores what sides might have been collided with
                CollisionSide potentialCollisionSide = CollisionSide.None;

                if (movingObjectPreviousHitbox.Right <= stationaryObjectHitbox.Left)
                {
                    //Did not collide with right side; might have collided with left side
                    potentialCollisionSide |= CollisionSide.Left;

                    cornerSlopeRun = stationaryObjectHitbox.Left - movingObjectPreviousHitbox.Right;

                    if (movingObjectPreviousHitbox.Bottom <= stationaryObjectHitbox.Top)
                    {
                        //Might have collided with top side
                        potentialCollisionSide |= CollisionSide.Top;
                        cornerSlopeRise =
                            stationaryObjectHitbox.Top - movingObjectPreviousHitbox.Bottom;
                    }
                    else if (movingObjectPreviousHitbox.Top >= stationaryObjectHitbox.Bottom)
                    {
                        //Might have collided with bottom side
                        potentialCollisionSide |= CollisionSide.Bottom;
                        cornerSlopeRise =
                            stationaryObjectHitbox.Bottom - movingObjectPreviousHitbox.Top;
                    }
                    else
                    {
                        //Did not collide with top side or bottom side or right side
                        return CollisionSide.Left;
                    }
                }
                else if (movingObjectPreviousHitbox.Left >= stationaryObjectHitbox.Right)
                {
                    //Did not collide with left side; might have collided with right side
                    potentialCollisionSide |= CollisionSide.Right;

                    cornerSlopeRun = movingObjectPreviousHitbox.Left - stationaryObjectHitbox.Right;

                    if (movingObjectPreviousHitbox.Bottom <= stationaryObjectHitbox.Top)
                    {
                        //Might have collided with top side
                        potentialCollisionSide |= CollisionSide.Top;
                        cornerSlopeRise =
                            movingObjectPreviousHitbox.Bottom - stationaryObjectHitbox.Top;
                    }
                    else if (movingObjectPreviousHitbox.Top >= stationaryObjectHitbox.Bottom)
                    {
                        //Might have collided with bottom side
                        potentialCollisionSide |= CollisionSide.Bottom;
                        cornerSlopeRise =
                            movingObjectPreviousHitbox.Top - stationaryObjectHitbox.Bottom;
                    }
                    else
                    {
                        //Did not collide with top side or bottom side or left side;
                        return CollisionSide.Right;
                    }
                }
                else
                {
                    //Did not collide with either left or right side;
                    //must be top side, bottom side, or none
                    if (movingObjectPreviousHitbox.Bottom <= stationaryObjectHitbox.Top)
                        return CollisionSide.Top;
                    else if (movingObjectPreviousHitbox.Top >= stationaryObjectHitbox.Bottom)
                        return CollisionSide.Bottom;
                    else
                        //Previous hitbox of moving object was already colliding with stationary object
                        return CollisionSide.None;
                }

                //Corner case; might have collided with more than one side
                //Compare slopes to see which side was collided with
                return GetCollisionSideFromSlopeComparison(
                    potentialCollisionSide,
                    velocitySlope,
                    cornerSlopeRise / cornerSlopeRun
                );
            }

            /// <summary>Gets which side of a stationary object was collided with by a moving object
            /// by comparing the slope of the moving object's velocity and the slope of the velocity
            /// that would have caused the moving object to be touching corners with the stationary
            /// object.</summary>
            /// <param name="potentialSides">The potential two sides that the moving object might have
            /// collided with.</param>
            /// <param name="velocitySlope">The slope of the moving object's velocity.</param>
            /// <param name="nearestCornerSlope">The slope of the path from the closest corner of the
            /// moving object's previous hitbox to the closest corner of the stationary object's
            /// hitbox.</param>
            /// <returns>The side of the stationary object with which the moving object collided.
            /// </returns>
            static CollisionSide GetCollisionSideFromSlopeComparison(
                CollisionSide potentialSides,
                double velocitySlope,
                double nearestCornerSlope
            )
            {
                if ((potentialSides & CollisionSide.Top) == CollisionSide.Top)
                {
                    if ((potentialSides & CollisionSide.Left) == CollisionSide.Left)
                        return velocitySlope < nearestCornerSlope
                            ? CollisionSide.Top
                            : CollisionSide.Left;
                    else if ((potentialSides & CollisionSide.Right) == CollisionSide.Right)
                        return velocitySlope > nearestCornerSlope
                            ? CollisionSide.Top
                            : CollisionSide.Right;
                }
                else if ((potentialSides & CollisionSide.Bottom) == CollisionSide.Bottom)
                {
                    if ((potentialSides & CollisionSide.Left) == CollisionSide.Left)
                        return velocitySlope > nearestCornerSlope
                            ? CollisionSide.Bottom
                            : CollisionSide.Left;
                    else if ((potentialSides & CollisionSide.Right) == CollisionSide.Right)
                        return velocitySlope < nearestCornerSlope
                            ? CollisionSide.Bottom
                            : CollisionSide.Right;
                }
                return CollisionSide.None;
            }

            /// <summary>Returns a Vector2 storing the correct location of a moving object
            /// after collision with a stationary object has been resolved.</summary>
            /// <param name="movingObjectHitbox">The hitbox of the moving object colliding with a
            /// stationary object.</param>
            /// <param name="stationaryObjectHitbox">The hitbox of the stationary object.</param>
            /// <param name="collisionSide">The side of the stationary object with which the moving
            /// object collided.</param>
            /// <returns>A Vector2 storing the corrected location of the moving object
            /// after resolving its collision with the stationary object.</returns>
            public static Vector2 GetCorrectedLocation(
                Rectangle movingObjectHitbox,
                Rectangle stationaryObjectHitbox,
                CollisionSide collisionSide
            )
            {
                Vector2 correctedLocation = movingObjectHitbox.Location.ToVector2();
                switch (collisionSide)
                {
                    case CollisionSide.Left:
                        correctedLocation.X = stationaryObjectHitbox.X - movingObjectHitbox.Width;
                        break;
                    case CollisionSide.Right:
                        correctedLocation.X =
                            stationaryObjectHitbox.X + stationaryObjectHitbox.Width;
                        break;
                    case CollisionSide.Top:
                        correctedLocation.Y = stationaryObjectHitbox.Y - movingObjectHitbox.Height;
                        break;
                    case CollisionSide.Bottom:
                        correctedLocation.Y =
                            stationaryObjectHitbox.Y + stationaryObjectHitbox.Height;
                        break;
                }
                return correctedLocation;
            }

            /// <summary>Returns the distance between the centers of two Rectangles.</summary>
            /// <param name="firstRectangle">The first Rectangle to compare.</param>
            /// <param name="secondRectangle">The second Rectangle to compare.</param>
            /// <returns>The distance between the centers of the two Rectangles.</returns>
            public static float GetDistance(Rectangle firstRectangle, Rectangle secondRectangle)
            {
                Vector2 firstCenter =
                    new(
                        firstRectangle.X + firstRectangle.Width / 2f,
                        firstRectangle.Y + firstRectangle.Height / 2f
                    );
                Vector2 secondCenter =
                    new(
                        secondRectangle.X + secondRectangle.Width / 2f,
                        secondRectangle.Y + secondRectangle.Height / 2f
                    );
                return Vector2.Distance(firstCenter, secondCenter);
            }

            /// <summary>Returns the squared distance between the centers of two Rectangles.</summary>
            /// <param name="firstRectangle">The first Rectangle to compare.</param>
            /// <param name="secondRectangle">The second Rectangle to compare.</param>
            /// <returns>The squared distance between the centers of the two Rectangles.</returns>
            public static float GetDistanceSquared(
                Rectangle firstRectangle,
                Rectangle secondRectangle
            )
            {
                Vector2 firstCenter =
                    new(
                        firstRectangle.X + firstRectangle.Width / 2f,
                        firstRectangle.Y + firstRectangle.Height / 2f
                    );
                Vector2 secondCenter =
                    new(
                        secondRectangle.X + secondRectangle.Width / 2f,
                        secondRectangle.Y + secondRectangle.Height / 2f
                    );
                return Vector2.DistanceSquared(firstCenter, secondCenter);
            }
        }
    }
}
