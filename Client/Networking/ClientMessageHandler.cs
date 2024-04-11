using Block2D.Common;
using Microsoft.Xna.Framework;
using Riptide;

namespace Block2D.Client.Networking
{
    public class ClientMessageHandler
    {
        public static void PlayerJoin()
        {
            Message message = Message.Create(MessageSendMode.Reliable, ClientMessageID.PlayerJoin);
            message.AddString(ClientMain.Username);
            ClientMain.Send(message);
        }

        public static void RequestChunk(Point position)
        {
            Message message = Message.Create(
                MessageSendMode.Unreliable,
                ClientMessageID.SendChunkRequest
            );
            message.AddPoint(position);
            message.AddString(ClientMain.LocalPlayer.Dimension);
            ClientMain.Send(message);
        }

        public static void SendPosition(Vector2 position)
        {
            Message message = Message.Create(
                MessageSendMode.Unreliable,
                ClientMessageID.SendPosition
            );
            message.AddVector2(position);
            ClientMain.Send(message);
        }

        [MessageHandler((ushort)ClientMessageID.ReceivePosition)]
        private static void HandlePosition(Message message)
        {
            Vector2 position = message.GetVector2();
            ushort id = message.GetUShort();

            if (ClientMain.CurrentWorld.Players.TryGetValue(id, out ClientPlayer player))
            {
                if (id == ClientMain.ID)
                {
                    return;
                }

                player.Position = position;
            }
        }

        [MessageHandler((ushort)ClientMessageID.HandlePlayerSpawn)]
        public static void PlayerJoin(Message message)
        {
            string name = message.GetString();
            Vector2 position = message.GetVector2();
            ushort id = message.GetUShort();

            ClientPlayer newPlayer = new(id, name) { Position = position, };

            ClientMain.CurrentWorld.AddPlayer(newPlayer);

            if (ClientMain.ID == id)
            {
                ClientMain.OnJoinWorld();
                RequestChunk(Point.Zero);
                RequestChunk(new(64, 0));
                RequestChunk(new(64, -64));
                RequestChunk(new(0, -64));
            }
        }

        [MessageHandler((ushort)ClientMessageID.ReceiveChunk)]
        public static void ReceiveChunk(Message message)
        {
            Point position = message.GetPoint();
            string dimension = message.GetString();
            byte offset = message.GetByte();
            byte[] compressedTiles = message.GetBytes();

            if (dimension != ClientMain.LocalPlayer.Dimension)
            {
                return;
            }

            byte[] decompressedTileBytes = compressedTiles.Decompress();

            ushort[] decompressedTiles = decompressedTileBytes.ToUShortArray();

            if (offset == 0)
            {
                ClientChunk newChunk = new(position, ClientMain.GetInstance());

                newChunk.SetSection(decompressedTiles, offset);
                ClientMain.CurrentWorld.TryAddChunk(newChunk);
            }
            else
            {
                if (ClientMain.CurrentWorld.GetChunkLoaded(position, out ClientChunk chunk))
                {
                    if (offset == chunk.ReceivedSections + 1 && offset < 4)
                    {
                        chunk.SetSection(decompressedTiles, offset);
                    }
                    else
                    {
                        ClientMain.LogWarning("Received Corrupted Chunk Data.");
                    }
                }
            }
        }
    }
}
