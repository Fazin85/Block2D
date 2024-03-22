using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;

namespace Block2D.Modding.DataStructures
{
    public struct ModItem
    {
        public int Damage { get; set; }
        public string Name { get; set; }
        public Rectangle Hitbox { get; set; }
        public Point Size { get; set; }
        public int Type { get; set; }
        public bool Stackable { get; set; }
        public int MaxStackSize { get; set; }
        public Script ItemCode { get; set; }
    }
}
