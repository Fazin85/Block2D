using Block2D.Common;
using Microsoft.Xna.Framework;
using Riptide;
using System;

namespace Block2D.Server.Networking
{
    public class ServerMessageHandler
    {
        [MessageHandler((ushort)MessageID.HandlePlayerJoin)]
        public static void HandlePlayerJoin(ushort fromClientId, Message message)
        {
            string clientPlayerName = message.GetString();

            ServerPlayer newPlayer = new(Vector2.Zero, 20, clientPlayerName);

            InternalServer.Instance.World.AddPlayer(newPlayer);
        }

        [MessageHandler((ushort)MessageID.HandleChunkRequest)]
        public static void HandleChunkRequest(ushort fromClientId, Message message)
        {
            Point position = message.GetPoint();
            string playerDimension = message.GetString();

            Message newMessage = Message.Create(MessageSendMode.Unreliable, MessageID.SendChunk);
            newMessage.AddPoint(position);
            newMessage.AddString(playerDimension);
            if (
                InternalServer
                    .Instance.World.Dimensions[playerDimension]
                    .ChunkManager.TryAddNewChunk(position, out Chunk newChunk)
            )
            {
                ushort[] tileIds = new ushort[Chunk.CHUNK_SIZE * Chunk.CHUNK_SIZE];

                for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
                {
                    for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                    {
                        Tile currentTile = newChunk.Tiles[x, y];

                        tileIds[Chunk.CHUNK_SIZE * x + y] = currentTile.ID;
                    }
                }
                byte[] target = new byte[tileIds.Length * 2];
                Buffer.BlockCopy(tileIds, 0, target, 0, tileIds.Length * 2);
                byte[] bytesToSend = Helper.Compress(target);
                newMessage.AddBytes(bytesToSend);
                InternalServer.Instance.Send(newMessage, fromClientId);
            }
        }


    }
}
