using Block2D.Common;
using Microsoft.Xna.Framework;
using Riptide;
using System.Diagnostics;

namespace Block2D.Server.Networking
{
    public class ServerMessageHandler
    {
        [MessageHandler((ushort)MessageID.HandlePlayerJoin)]
        public static void HandlePlayerJoin(ushort fromClientId, Message message)
        {
            string clientPlayerName = message.GetString();

            ServerPlayer newPlayer = new(Vector2.Zero, 20, clientPlayerName);

            Main.InternalServer.World.AddPlayer(newPlayer);
        }

        [MessageHandler((ushort)MessageID.HandleChunkRequest)]
        public static void HandleChunkRequest(ushort fromClientId, Message message)
        {
            Point position = message.GetPoint();
            string playerDimension = message.GetString();

            if (
                Main
                    .InternalServer.World.Dimensions[playerDimension]
                    .ChunkManager.TryAddNewChunk(position, out ServerChunk newChunk)
            )
            {
                for (byte i = 0; i < 4; i++)
                {
                    Message newMessage = Message.Create(MessageSendMode.Unreliable, MessageID.SendChunk);
                    newMessage.AddPoint(position);
                    newMessage.AddString(playerDimension);
                    newMessage.AddByte(i);

                    byte[] tileBytes = newChunk.Sections[i].Tiles.ToByteArray();
                    byte[] bytesToSend = tileBytes.Compress();

                    newMessage.AddBytes(bytesToSend);
                    Main.InternalServer.Send(newMessage, fromClientId);
                }
            }
        }
    }
}
