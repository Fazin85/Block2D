using Block2D.Common;
using Microsoft.Xna.Framework;
using Riptide;
using System.Diagnostics;

namespace Block2D.Client.Networking
{
    public class ClientMessageHandler
    {
        public void PlayerJoin()
        {
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.PlayerJoin);
            message.AddString("Hello World");
            Main.Client.Send(message);
            RequestChunk(new(64, 0));
        }

        public void RequestChunk(Point position)
        {
            Message message = Message.Create(
                MessageSendMode.Unreliable,
                MessageID.SendChunkRequest
            );
            message.AddPoint(position);
            message.AddString(Main.Client.LocalPlayer.Dimension);
            Main.Client.Send(message);
        }

        [MessageHandler((ushort)MessageID.ReceiveChunk)]
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
            Debug.WriteLine(compressedTiles.Length);

            if (offset == 0)
            {
                ClientChunk newChunk = new(position);

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
