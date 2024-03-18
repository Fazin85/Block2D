namespace Block2D.Common
{
    public struct Tile
    {
        public readonly ushort ID
        {
            get => _id;
        }
        private ushort _id;
        public bool Modded;

        public Tile()
        {
            _id = 0;
            Modded = false;
        }

        public Tile(bool modded)
        {
            _id = 0;
            Modded = modded;
        }

        public void Tick(int x, int y, string dimensionId)
        {
            if (Modded)
            {
                
            }
            else
            {

            }
        }

        public void Set(ushort id)
        {
            _id = id;
        }
    }
}
