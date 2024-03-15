using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Block2D
{
    public class FPSCounter
    {
        private double frames = 0;
        private double updates = 0;
        private double elapsed = 0;
        private double last = 0;
        private double now = 0;
        public const double MSG_FREQUENCY = 1.0f;
        public string msg = "";

        /// <summary>
        /// The msgFrequency here is the reporting time to update the message.
        /// </summary>
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
                    + frames.ToString();
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
            Vector2 fpsDisplayPosition,
            Color fpsTextColor
        )
        {
            spriteBatch.DrawString(font, msg, fpsDisplayPosition, fpsTextColor);
            frames++;
        }

        public void Reset()
        {
            now = 0;
            elapsed = 0;
            last = 0;
            frames = 0;
            updates = 0;
        }
    }
}
