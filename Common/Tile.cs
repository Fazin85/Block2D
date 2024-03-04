namespace Block2D.Common
{
    public struct Tile : ITickable
    {
        public readonly ushort ID
        {
            get => _id;
        }
        private ushort _id;

        public Tile()
        {
            _id = 0;
        }

        public void Tick() { }

        public void Set(ushort id)
        {
            _id = id;
        }
    }
}
