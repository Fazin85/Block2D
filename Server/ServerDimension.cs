namespace Block2D.Server
{
    public class ServerDimension
    {
        public ServerChunkManager ChunkManager { get; private set; }

        public string Name { get; private set; }

        public int MaxChunksX { get; private set; }

        public int MaxChunksY { get; private set; }

        public int MaxEntitys { get; private set; }

        public int MaxDroppedItems { get; private set; }

        public ServerDimension(InternalServer server, string name, int seed, int maxChunksX, int maxChunksY, int maxEntites, int maxDroppedItems)
        {
            ChunkManager = new(server, name, seed);
            Name = name;
            MaxChunksX = maxChunksX;
            MaxChunksY = maxChunksY;
            MaxEntitys = maxEntites;
            MaxDroppedItems = maxDroppedItems;
        }
    }
}
