namespace Block2D.Common
{
    public struct Tile
    {
        public ushort ID { get; private set; }
        public bool Tickable { get; private set; }
        public bool TileEntity { get; private set; }

        public readonly void Tick(int x, int y, string dimensionId)
        {
            bool randomTick = Main.Random.Next(160) == 0;

            string name = Main.InternalServer.World.GetTileName(ID);

            if (TileEntity)
            {

            }
            //else if (randomTick)
            //{
            //    string name = Main.InternalServer.World.GetTileName(ID);

            //    if (name == TileID.DIRT)
            //    {
            //        Debug.WriteLine("Ticked Dirt");
            //    }

            //    var modTile = Main.AssetManager.GetTile(name);

            //    if (modTile.Tickable)
            //    {
            //        modTile.Script.Call(modTile.Script.Globals["RandomTick"], x, y, dimensionId);
            //    }
            //}
        }

        public void Set(ushort id, bool tickable, bool tileEntity)
        {
            ID = id;
            Tickable = tickable;
            TileEntity = tileEntity;
        }
    }
}
