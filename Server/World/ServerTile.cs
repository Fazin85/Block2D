using Block2D.Common;
using Block2D.Modding.DataStructures;

namespace Block2D.Server.World
{
    public struct ServerTile
    {
        public ushort ID { get; private set; }
        public bool Tickable { get; private set; }
        public bool TileEntity { get; private set; }
        public bool Collidable { get; private set; }

        private readonly ServerChunk _containerChunk;

        public ServerTile(ServerChunk chunk)
        {
            _containerChunk = chunk;
        }

        public readonly void Tick(int x, int y, string dimensionId)
        {
            if (Tickable && Main.Random.Next(160) == 0)
            {
                string name = _containerChunk.Server.World.GetTileName(ID);
                ModTile tile = _containerChunk.Server.AssetManager.GetTile(name);

                tile.TileCode.Call(tile.TileCode.Globals["Tick"], x, y, dimensionId);
            }
        }

        public void Set(ushort id, bool tickable, bool tileEntity, bool collidable)
        {
            ID = id;
            Tickable = tickable;
            TileEntity = tileEntity;
            Collidable = collidable;
        }
    }
}
