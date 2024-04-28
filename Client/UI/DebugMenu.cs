using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Block2D.Client.UI
{
    public class DebugMenu
    {
        private double frames = 0;
        private double updates = 0;
        private double elapsed = 0;
        private double last = 0;
        private double now = 0;
        public const double MSG_FREQUENCY = 1.0f;
        private string msg = "";
        public int ChunksToRenderCount = 0;

        public void Update(GameTime gameTime)
        {
            now = gameTime.TotalGameTime.TotalSeconds;
            elapsed = now - last;
            if (elapsed > MSG_FREQUENCY)
            {
                int fps = (int)(frames / elapsed) + 1;

                msg =
                    " Fps: "
                    + fps.ToString()
                    + "\n Elapsed time: "
                    + elapsed.ToString()
                    + "\n Updates: "
                    + updates.ToString()
                    + "\n Frames: "
                    + frames.ToString()
                    + "\n ChunksToRenderCount: "
                    + ChunksToRenderCount.ToString();
                elapsed = 0;
                frames = 0;
                updates = 0;
                last = now;
            }
            updates++;
        }

        public void Draw(
            SpriteBatch spriteBatch,
            SpriteFont font,
            Vector2 displayPosition,
            Color textColor
        )
        {
            spriteBatch.DrawString(font, msg, displayPosition, textColor);
            frames++;
        }

        public void Reset()
        {
            now = 0;
            elapsed = 0;
            last = 0;
            frames = 0;
            updates = 0;
            ChunksToRenderCount = 0;
        }
    }
}
