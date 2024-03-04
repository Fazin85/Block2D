using Microsoft.Xna.Framework;

namespace Block2D.Common
{
    public static class Collision
    {
        public static bool CollidingWithTile(Rectangle playerHitbox, int x, int y)
        {
            Rectangle tileHitbox = new(x, y, 16, 16);
            return playerHitbox.Intersects(tileHitbox);
        }

        public static bool CollidingWithTile(Rectangle playerHitbox, Vector2 position)
        {
            Rectangle tileHitbox = new((int)position.X, (int)position.Y, 16, 16);
            return playerHitbox.Intersects(tileHitbox);
        }

        public static bool CollidingWithTiles(Point position, Point velocity, Point size)
        {
            Rectangle hitbox = new(position + velocity, size);
            for (int x = hitbox.Left; x <= hitbox.Right; x++)
            {
                for (int y = hitbox.Top; y <= hitbox.Bottom; y++)
                {
                    Point currentPosition = new(x, y);
                    if (Main.Client.World.TryGetTile(currentPosition, out Tile tile))
                    {
                        if (tile.Collidable())
                        {
                            Rectangle currentTileRect = new(currentPosition, new(16, 16));
                            if (hitbox.Intersects(currentTileRect))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
