using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;

namespace Block2D.Modding.DataStructures
{
    public struct ModTile
    {
        public string Name { get; set; }
        public string TextureName { get; set; }
        public float TextureScale { get; set; }
        public string HitSoundEffectName { get; set; }
        public bool Tickable { get; set; }
        public Color DrawColor { get; set; }
        public Script TileCode { get; set; }
    }
}
