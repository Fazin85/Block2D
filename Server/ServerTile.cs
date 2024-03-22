using Block2D.Common;
using Block2D.Modding.DataStructures;

namespace Block2D.Server
{
    public struct ServerTile
    {
        public ushort ID { get; private set; }
        public bool Tickable { get; private set; }
        public bool TileEntity { get; private set; }

        public readonly void Tick(int x, int y, string dimensionId)
        {
            if (Tickable && Main.Random.Next(160) == 0)
            {
                string name = Main.InternalServer.World.GetTileName(ID);
                ModTile tile = Main.AssetManager.GetTile(name);

                tile.TileCode.Call(tile.TileCode.Globals["Tick"], x, y, dimensionId);
            }
        }

        public void Set(ushort id, bool tickable, bool tileEntity)
        {
            ID = id;
            Tickable = tickable;
            TileEntity = tileEntity;
        }
    }
}
