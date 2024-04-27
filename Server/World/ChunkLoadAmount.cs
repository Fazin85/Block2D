namespace Block2D.Server.World
{
    public enum ChunkLoadAmount
    {
        Unloaded, //the chunk is still in the chunk dict but isn't being ticked.
        Ticking, //the chunk is being ticked but isn't visible to any players.
        FullyLoaded //the chunk is being ticked and is visible to players.
    }
}
