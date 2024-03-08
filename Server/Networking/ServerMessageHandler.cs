using Block2D.Common;
using Microsoft.Xna.Framework;
using Riptide;

namespace Block2D.Server.Networking
{
    public class ServerMessageHandler
    {
        [MessageHandler((ushort)ServerMessageID.HandlePlayerJoin)]
        public static void HandlePlayerJoin(ushort fromClientId, Message message)
        {
            string clientPlayerName = message.GetString();

            ServerPlayer newPlayer = new(fromClientId, -Vector2.UnitY * 16, 20, clientPlayerName);

            foreach (ServerPlayer otherPlayer in Main.InternalServer.World.Players.Values)
            {
                Main.InternalServer.Send(CreateSpawnMessage(otherPlayer), fromClientId);
            }

            Main.InternalServer.World.AddPlayer(newPlayer);
            Main.InternalServer.SendToAll(CreateSpawnMessage(newPlayer));
        }

        public static Message CreateSpawnMessage(ServerPlayer player)
        {
            Message message = Message.Create(MessageSendMode.Reliable, ServerMessageID.PlayerSpawn);
            message.AddString(player.Name);
            message.AddVector2(player.Position);
            message.AddUShort(player.ID);
            return message;
        }

        public static void SendPositions()
        {
            foreach (ServerPlayer player in Main.InternalServer.World.Players.Values)
            {
                Message message = Message.Create(
                    MessageSendMode.Unreliable,
                    ServerMessageID.SendPosition
                );
                message.AddVector2(player.Position);
                message.AddUShort(player.ID);
                Main.InternalServer.SendToAll(message);
            }
        }

        [MessageHandler((ushort)ServerMessageID.ReceivePosition)]
        public static void ReceivePosition(ushort fromClientId, Message message)
        {
            Vector2 position = message.GetVector2();
            if (
                Main.InternalServer.World.Players.TryGetValue(fromClientId, out ServerPlayer player)
            )
            {
                player.Position = position;
            }
        }

        [MessageHandler((ushort)ServerMessageID.HandleChunkRequest)]
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
                    Message newMessage = Message.Create(
                        MessageSendMode.Unreliable,
                        ServerMessageID.SendChunk
                    );
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
