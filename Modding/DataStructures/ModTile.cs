using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;

namespace Block2D.Modding.DataStructures
{
    public readonly struct ModTile
    {
        public readonly string Name
        {
            get => _name;
        }

        public readonly string TextureName
        {
            get => _textureName;
        }

        public readonly float TextureScale
        {
            get => _textureScale;
        }

        public readonly string HitSoundEffectName
        {
            get => _hitSoundEffectName;
        }

        public readonly bool Tickable
        {
            get => _tickable;
        }

        public readonly bool Collidable
        {
            get => _collidable;
        }

        public readonly Color DrawColor
        {
            get => _drawColor;
        }

        public readonly Script TileCode
        {
            get => _tileCode;
        }

        private readonly string _name;
        private readonly string _textureName;
        private readonly float _textureScale;
        private readonly string _hitSoundEffectName;
        private readonly bool _tickable;
        private readonly bool _collidable;
        private readonly Color _drawColor;
        private readonly Script _tileCode;

        public ModTile(string name, string textureName, float textureScale, string hitSoundEffectName, bool tickable, bool collidable, Color drawColor, Script tileCode)
        {
            _name = name;
            _textureName = textureName;
            _textureScale = textureScale;
            _hitSoundEffectName = hitSoundEffectName;
            _tickable = tickable;
            _collidable = collidable;
            _drawColor = drawColor;
            _tileCode = tileCode;
        }
    }
}
