using Block2D.Common;
using Block2D.Modding.DataStructures;
using Microsoft.Xna.Framework;

namespace Block2D.Server
{
    public class ServerChunk
    {
        public ChunkLoadAmount LoadAmount { get; set; }

        public ChunkSectionData[] Sections
        {
            get => ChunkSectionData.GetChunkSections(this);
        }

        public Point Position { get; private set; }
        public InternalServer Server { get; private set; }

        private readonly ServerTile[,] _tiles;
        private readonly string _dimensionId;

        public ServerChunk(Point position, string dimensionId, InternalServer server)
        {
            _tiles = new ServerTile[CC.CHUNK_SIZE, CC.CHUNK_SIZE];
            LoadAmount = ChunkLoadAmount.Unloaded;
            Position = position;
            _dimensionId = dimensionId;
            Server = server;
        }

        public void SetTile(Point position, string tileName)
        {
            if (!Server.World.LoadedTiles.ContainsKey(tileName))
            {
                return;
            }

            if (position.X >= CC.CHUNK_SIZE || position.X < 0)
            {
                Server.LogWarning("Position X is out of bounds");
                return;
            }

            if (position.Y >= CC.CHUNK_SIZE || position.Y < 0)
            {
                Server.LogWarning("Position Y is out of bounds");
                return;
            }

            ushort id = Server.World.LoadedTiles[tileName];

            ModTile tile = Server.AssetManager.GetTile(tileName);

            //TODO add tileEntitys
            _tiles[position.X, position.Y].Set(id, tile.Tickable, false, tile.Collidable);
        }

        public ServerTile GetTile(Point position)
        {
            if (position.X >= CC.CHUNK_SIZE || position.X < 0)
            {
                Server.LogWarning("Position X is out of bounds");
            }

            if (position.Y >= CC.CHUNK_SIZE || position.Y < 0)
            {
                Server.LogWarning("Position Y is out of bounds");
            }

            return _tiles[position.X, position.Y];
        }

        public void Tick()
        {
            //var watch = System.Diagnostics.Stopwatch.StartNew();
            for (int x = 0; x < CC.CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CC.CHUNK_SIZE; y++)
                {
                    _tiles[x, y].Tick(Position.X + x, Position.Y + y, _dimensionId);
                }
            }
            //watch.Stop();
            //var elapsedMs = watch.ElapsedMilliseconds;
            //Debug.WriteLine(elapsedMs.ToString());
        }
    }
}
