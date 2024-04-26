using Microsoft.Xna.Framework.Input;

namespace Block2D.Client
{
    public static class KeyExtensions
    {
        public static bool IsNumber(this Keys key, out int number)
        {
            number = 0;

            if (key == Keys.D1 || key == Keys.D2 || key == Keys.D3 || key == Keys.D4 || key == Keys.D5 || key == Keys.D6 || key == Keys.D7 || key == Keys.D8 || key == Keys.D9 || key == Keys.D0)
            {
                number = int.Parse(key.ToString().Remove(0, 1));//remove D from the string
                return true;
            }

            return false;
        }
    }
}
