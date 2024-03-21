namespace Block2D.Client
{
    public struct ClientTile
    {
        public ushort ID { get; private set; }
        public bool TileEntity { get; private set; }

        public void Set(ushort id, bool tileEntity)
        {
            ID = id;
            TileEntity = tileEntity;
        }
    }
}
