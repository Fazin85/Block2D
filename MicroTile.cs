namespace Block2D
{
    public struct MicroTile : ITickable
    {
        private ushort _id;

        public void Tick() { }

        public void Set(ushort id)
        {
            _id = id;
        }
    }
}
