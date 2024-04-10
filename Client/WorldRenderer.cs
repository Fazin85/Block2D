﻿using Block2D.Common;
using Block2D.Common.ID;
using Block2D.Modding.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Block2D.Client
{
    public class WorldRenderer
    {
        public void DrawChunks(
            ClientChunk[] chunksToDraw,
            SpriteBatch spriteBatch,
            RectangleF viewport
        )
        {
            int chunksToRenderCount = 0;

            for (int i = 0; i < chunksToDraw.Length; i++)
            {
                ClientChunk currentChunk = chunksToDraw[i];

                RectangleF currentChunkRect =
                    new(
                        new(
                            currentChunk.Position.X * CC.TILE_SIZE,
                            currentChunk.Position.Y * CC.TILE_SIZE
                        ),
                        new(CC.CHUNK_SIZE * CC.TILE_SIZE, CC.CHUNK_SIZE * CC.TILE_SIZE)
                    ); //create a rectangle the size of a chunk

                if (currentChunk.ReceivedSections != 3 || !viewport.Intersects(currentChunkRect)) //don't draw chunks out of the camera's view
                {
                    continue;
                }

                chunksToRenderCount++;

                for (int x = 0; x < CC.CHUNK_SIZE; x++)
                {
                    for (int y = 0; y < CC.CHUNK_SIZE; y++)
                    {
                        Point currentTilePosition = new(x, y);

                        ClientTile currentTile = currentChunk.GetTile(currentTilePosition);

                        Vector2 positionToDrawBlock =
                            (currentChunk.Position.ToVector2() + currentTilePosition.ToVector2())
                            * CC.TILE_SIZE;

                        if (!viewport.Contains(positionToDrawBlock)) //don't draw tiles out of the camera's view
                        {
                            continue;
                        }

                        string currentTileName = ClientMain.GetTileName(currentTile.ID);

                        if (currentTileName != TileID.AIR)
                        {
                            ModTile tile = Main.AssetManager.GetTile(currentTileName);
                            spriteBatch.Draw(
                                Main.AssetManager.GetTexture(currentTileName),
                                positionToDrawBlock,
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

                if (ClientMain.DebugMode)
                {
                    spriteBatch.DrawRectangle(currentChunkRect, Color.Red);

                    ClientMain.DebugMenu.ChunksToRenderCount = chunksToRenderCount;
                }
            }
        }
    }
}
