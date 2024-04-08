using Microsoft.Xna.Framework.Graphics;

namespace Block2D.Modding.DataStructures
{
    public readonly struct ModTexture
    {
        public readonly string Name
        {
            get => _name;
        }
        public readonly Texture2D Texture
        {
            get => _texture;
        }

        private readonly string _name;
        private readonly Texture2D _texture;

        public ModTexture(string name, Texture2D texture)
        {
            _name = name;
            _texture = texture;
        }
    }
}
