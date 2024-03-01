using System;
using Microsoft.Xna.Framework;

namespace Block2D
{
    public struct Tile : ITickable
    {
        public bool HasMicroTile
        {
            get => _hasMicroTile;
        }

        public ushort ID
        {
            get => _id;
        }

        private MicroTile[] _microTiles;
        private ushort _id;
        private bool _hasMicroTile;

        public Tile()
        {
            _microTiles = new MicroTile[4];
            _hasMicroTile = false;
            _id = 0;
        }

        public void Tick()
        {
            if (_hasMicroTile)
            {
                for (int i = 0; i < _microTiles.Length; i++)
                {
                    _microTiles[i].Tick();
                }
            }
        }

        public void Set(ushort id)
        {
            if (_hasMicroTile)
            {
                return;
            }

            _id = id;
            _hasMicroTile = false;
        }

        public void Set(int microTilePosition, ushort microTileId)
        {
            if (_id != 0 || _id != ushort.MaxValue)
            {
                return;
            }

            if (microTilePosition > 3 || microTilePosition < 0)
            {
                Console.WriteLine("microTilePosition is out of bounds");
                return;
            }

            _hasMicroTile = true;
            _id = ushort.MaxValue; //ushort max values signifies that the you can place a microtile in this tile
            _microTiles[microTilePosition].Set(microTileId);
        }
    }
}
