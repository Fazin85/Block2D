using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Block2D.Client
{
    public class Renderer
    {
        public static void DrawChunks(
            Chunk[] chunksToDraw,
            SpriteBatch spriteBatch,
            AssetManager assets
        )
        {
            for (int i = 0; i < chunksToDraw.Length; i++)
            {
                Chunk currentChunk = chunksToDraw[i];

                for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
                {
                    for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                    {
                        Vector2 currentTilePosition = new(x, y);
                        Tile currentTile = currentChunk.GetTile(currentTilePosition);

                        switch (currentTile.ID)
                        {
                            case 0:
                                break;
                            case 1:
                                spriteBatch.Draw(
                                    assets.GetBlockTexture("Stone_Block"),
                                    currentChunk.Position + (currentTilePosition * 16),
                                    Color.White
                                );
                                break;
                            case 2:
                                spriteBatch.Draw(
                                    assets.GetBlockTexture("Grass_Block"),
                                    currentChunk.Position + (currentTilePosition * 16),
                                    Color.White
                                );
                                break;
                            case 3:
                                spriteBatch.Draw(
                                    assets.GetBlockTexture("Dirt_Block"),
                                    currentChunk.Position + (currentTilePosition * 16),
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
    }
}
