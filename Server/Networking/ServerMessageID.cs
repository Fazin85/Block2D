namespace Block2D.Server.Networking
{
    public enum ServerMessageID : ushort
    {
        HandlePlayerJoin,
        HandleChunkRequest,
        SendChunk,
        PlayerSpawn,
        ReceivePosition,
        SendPosition,
        ReceiveChatMessage
    }
}
