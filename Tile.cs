using System;
using Microsoft.Xna.Framework;

namespace Block2D
{
    public struct Tile : ITickable
    {
        public readonly ushort ID
        {
            get => _id;
        }
        private ushort _id;
        private bool _hasMicroTile;

        public Tile()
        {
            _hasMicroTile = false;
            _id = 0;
        }

        public void Tick() { }

        public void Set(ushort id)
        {
            if (_hasMicroTile)
            {
                return;
            }

            _id = id;
            _hasMicroTile = false;
        }
    }
}
