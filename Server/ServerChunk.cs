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

        private readonly ServerTile[,] _tiles;
        private readonly string _dimensionId;
        private readonly World _world;

        public ServerChunk(Point position, string dimensionId, World world)
        {
            _tiles = new ServerTile[CC.CHUNK_SIZE, CC.CHUNK_SIZE];
            LoadAmount = ChunkLoadAmount.Unloaded;
            Position = position;
            _dimensionId = dimensionId;
            _world = world;
        }

        public void SetTile(Point position, string tileName)
        {
            if (!_world.LoadedTiles.ContainsKey(tileName))
            {
                return;
            }

            if (position.X >= CC.CHUNK_SIZE || position.X < 0)
            {
                Main.Logger.Fatal("Position X is out of bounds");
                return;
            }

            if (position.Y >= CC.CHUNK_SIZE || position.Y < 0)
            {
                Main.Logger.Fatal("Position Y is out of bounds");
                return;
            }

            ushort id = _world.LoadedTiles[tileName];

            ModTile tile = Main.AssetManager.GetTile(tileName);

            //TODO add tileEntitys
            _tiles[position.X, position.Y].Set(id, tile.Tickable, false);
        }

        public ServerTile GetTile(Point position)
        {
            if (position.X >= CC.CHUNK_SIZE || position.X < 0)
            {
                Main.Logger.Fatal("Position X is out of bounds");
            }

            if (position.Y >= CC.CHUNK_SIZE || position.Y < 0)
            {
                Main.Logger.Fatal("Position Y is out of bounds");
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
