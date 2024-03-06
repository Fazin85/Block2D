using Block2D.Common;
using Microsoft.Xna.Framework;
using Riptide;

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
            byte[] compressedTiles = message.GetBytes();

            if (dimension != Main.Client.LocalPlayer.Dimension)
            {
                return;
            }

            byte[] decompressedTiles = compressedTiles.Decompress();

            ushort[] target = decompressedTiles.ToUShortArray();

            Chunk newChunk = new(position);

            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
            {
                for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                {
                    newChunk.SetTile(new Vector2(x, y), target[Chunk.CHUNK_SIZE * x + y]);
                }
            }

            Main.Client.World.TryAddChunk(newChunk);
        }
    }
}
