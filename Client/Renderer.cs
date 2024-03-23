using Block2D.Common;
using Block2D.Common.ID;
using Block2D.Modding.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Block2D.Client
{
    public class Renderer
    {
        public static void DrawChunks(ClientChunk[] chunksToDraw, SpriteBatch spriteBatch)
        {
            for (int i = 0; i < chunksToDraw.Length; i++)
            {
                ClientChunk currentChunk = chunksToDraw[i];

                if (currentChunk.ReceivedSections != 3)
                {
                    continue;
                }

                for (int x = 0; x < CC.CHUNK_SIZE; x++)
                {
                    for (int y = 0; y < CC.CHUNK_SIZE; y++)
                    {
                        Point currentTilePosition = new(x, y);
                        ClientTile currentTile = currentChunk.GetTile(currentTilePosition);

                        Vector2 positionToRenderBlock =
                            (currentChunk.Position.ToVector2() + currentTilePosition.ToVector2())
                            * CC.TILE_SIZE;

                        string currentTileName = Main.Client.GetTileName(currentTile.ID);

                        if (currentTileName != TileID.AIR)
                        {
                            ModTile tile = Main.AssetManager.GetTile(currentTileName);
                            spriteBatch.Draw(
                                Main.AssetManager.GetTexture(currentTileName),
                                positionToRenderBlock,
                                null,
                                tile.DrawColor,
                                0,
                                Vector2.Zero,
                                tile.TextureScale,
                                SpriteEffects.None,
                                0
                            );
                        }
                    }
                }
                if (Main.Client.DebugMode)
                {
                    RectangleF rectangle = new(new(currentChunk.Position.X * CC.TILE_SIZE, currentChunk.Position.Y *  CC.TILE_SIZE), new(CC.CHUNK_SIZE * CC.TILE_SIZE, CC.CHUNK_SIZE * CC.TILE_SIZE));

                    spriteBatch.DrawRectangle(rectangle, Color.Red);
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
