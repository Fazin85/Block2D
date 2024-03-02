using Microsoft.Xna.Framework;
using System.Collections.Generic;

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
        //TODO: ADD RENDERING FOR POSITIONS WHERE TILE COLLISION IS BEING CHECKED
        public static bool CollidingWithTiles(Rectangle hitbox)
        {
            int left = hitbox.Left;
            int right = hitbox.Right;
            int top = hitbox.Top;
            int bottom = hitbox.Bottom;

            List<bool> collidingTilePositions = new();

            for (int x = left; x <= right; x++)
            {
                for (int y = top; y <= bottom; y++)
                {
                    Vector2 hitBoxPosition = new(x, y);
                    if (
                        Main.Client.World.GetChunkLoaded(
                            hitBoxPosition.ToChunkCoords(),
                            out Chunk chunk
                        )
                    )
                    {
                        for (int x2 = 0; x2 < Chunk.CHUNK_SIZE; x2++)
                        {
                            for (int y2 = 0; y2 < Chunk.CHUNK_SIZE; y2++)
                            {
                                Tile currentTile = chunk.GetTile(new(x2, y2));

                                if (currentTile.ID == 0)
                                {
                                    collidingTilePositions.Add(false);
                                    break;
                                }

                                Point p =
                                    new(
                                        (int)chunk.Position.X + x2 * 16,
                                        (int)chunk.Position.Y + y2 * 16
                                    );

                                Rectangle tileHitbox = new(p, new(16, 16));

                                collidingTilePositions.Add(hitbox.Intersects(tileHitbox));
                            }
                        }
                        return collidingTilePositions.Contains(true);
                    }
                }
            }

            return false;
        }
    }
}
