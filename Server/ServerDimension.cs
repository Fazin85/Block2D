namespace Block2D.Server
{
    public class ServerDimension
    {
        public ServerChunkProvider ChunkProvider { get; private set; }
        
        public string Name { get; private set; }

        public int MaxChunksX { get; private set; }

        public int MaxChunksY { get; private set; }

        public int MaxEntites {  get; private set; }

        public int MaxDroppedItems { get; private set; }

        public ServerDimension(string name, int seed, int maxChunksX, int maxChunksY, int maxEntites, int maxDroppedItems)
        {
            ChunkProvider = new(name, seed);
            Name = name;
            MaxChunksX = maxChunksX;
            MaxChunksY = maxChunksY;
            MaxEntites = maxEntites;
            MaxDroppedItems = maxDroppedItems;
        }
    }
}
