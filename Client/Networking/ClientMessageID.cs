namespace Block2D.Client.Networking
{
    public enum ClientMessageID : ushort
    {
        PlayerJoin,
        SendChunkRequest,
        ReceiveChunk,
        HandlePlayerSpawn,
        SendPosition,
        ReceivePosition,
        SendChatMessage
    }
}
