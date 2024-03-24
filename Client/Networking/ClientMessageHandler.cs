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
            message.AddString(Main.Client.Username);
            Main.Client.Send(message);
        }

        public static void RequestChunk(Point position)
        {
            Message message = Message.Create(
                MessageSendMode.Unreliable,
                ClientMessageID.SendChunkRequest
            );
            message.AddPoint(position);
            message.AddString(Main.Client.LocalPlayer.Dimension);
            Main.Client.Send(message);
        }

        public static void SendPosition(Vector2 position)
        {
            Message message = Message.Create(
                MessageSendMode.Unreliable,
                ClientMessageID.SendPosition
            );
            message.AddVector2(position);
            Main.Client.Send(message);
        }

        [MessageHandler((ushort)ClientMessageID.ReceivePosition)]
        private static void HandlePosition(Message message)
        {
            Vector2 position = message.GetVector2();
            ushort id = message.GetUShort();

            if (Main.Client.World.Players.TryGetValue(id, out ClientPlayer player))
            {
                if (id == Main.Client.ID)
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

            Main.Client.World.AddPlayer(newPlayer);

            if (Main.Client.ID == id)
            {
                Main.Client.InWorld = true;
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

            if (dimension != Main.Client.LocalPlayer.Dimension)
            {
                return;
            }

            byte[] decompressedTileBytes = compressedTiles.Decompress();

            ushort[] decompressedTiles = decompressedTileBytes.ToUShortArray();

            if (offset == 0)
            {
                ClientChunk newChunk = new(position, Main.Client);

                newChunk.SetSection(decompressedTiles, offset);
                Main.Client.World.TryAddChunk(newChunk);
            }
            else
            {
                if (Main.Client.World.GetChunkLoaded(position, out ClientChunk chunk))
                {
                    if (offset == chunk.ReceivedSections + 1 && offset < 4)
                    {
                        chunk.SetSection(decompressedTiles, offset);
                    }
                }
            }
        }
    }
}
