using Block2D.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Block2D.Client
{
    public class Renderer
    {
        public static void DrawChunks(
            ClientChunk[] chunksToDraw,
            SpriteBatch spriteBatch,
            AssetManager assets
        )
        {
            for (int i = 0; i < chunksToDraw.Length; i++)
            {
                ClientChunk currentChunk = chunksToDraw[i];

                if (currentChunk.ReceivedSections != 3)
                {
                    continue;
                }

                for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
                {
                    for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                    {
                        Point currentTilePosition = new(x, y);
                        Tile currentTile = currentChunk.GetTile(currentTilePosition);

                        Vector2 positionToRenderBlock =
                            (currentChunk.Position.ToVector2() + currentTilePosition.ToVector2())
                            * 16;

                        switch (currentTile.ID)
                        {
                            case 0:
                                break;
                            case 1:
                                spriteBatch.Draw(
                                    assets.GetBlockTexture("Stone_Block"),
                                    positionToRenderBlock,
                                    Color.White
                                );
                                break;
                            case 2:
                                spriteBatch.Draw(
                                    assets.GetBlockTexture("Grass_Block"),
                                    positionToRenderBlock,
                                    Color.White
                                );
                                break;
                            case 3:
                                spriteBatch.Draw(
                                    assets.GetBlockTexture("Dirt_Block"),
                                    positionToRenderBlock,
                                    Color.White
                                );
                                break;
                            case 4:
                                break;
                        }
                    }
                }
            }
        }

        public static void DrawPlayer(
            ClientPlayer player,
            SpriteBatch spriteBatch,
            AssetManager assets
        )
        {
            spriteBatch.Draw(assets.GetPlayerTexture(), player.Position, Color.White);

            if (player.ID != Main.Client.ID)
            {
                return;
            }

            spriteBatch.DrawRectangle(player.Hitbox.ToRectangleF(), Color.Red);

            Rectangle nextHitbox = player.Hitbox;
            nextHitbox.Location += player.Velocity.ToPoint();
            spriteBatch.DrawRectangle(nextHitbox.ToRectangleF(), Color.Blue);
        }
    }
}
