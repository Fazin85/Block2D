using Microsoft.Xna.Framework;

namespace Block2D.Client
{
    public class Item
    {
        public int Damage { get; set; }
        public string Name { get; set; }
        public Rectangle Hitbox { get; set; }
        public int Width { get; set; }
        public Point Size { get; set; }
        public bool Stackable { get; set; }
        public int MaxStackSize { get; set; }

        public void OnUse()
        {

        }

        public bool CanUseItem(ClientPlayer player)
        {
            return false;
        }

        public void OnHitPlayer()
        {

        }

        public void OnHitTile()
        {

        }

        public void OnHitNPC()
        {

        }
    }
}
