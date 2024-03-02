﻿using Block2D.Common;
using Block2D.Common.ID;
using Microsoft.Xna.Framework;
using Riptide;
using System;

namespace Block2D.Client.Networking
{
    public class ClientMessageHandler
    {
        public void PlayerJoin()
        {
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.PlayerJoin);
            message.AddString("Hello World");
            Main.Client.Send(message);
            RequestChunk(new(1, 0));
        }

        public void RequestChunk(Vector2 position)
        {
            Message message = Message.Create(
                MessageSendMode.Unreliable,
                MessageID.SendChunkRequest
            );
            message.AddVector2(position);
            message.AddString(Main.Client.LocalPlayer.Dimension);
            Main.Client.Send(message);
        }

        [MessageHandler((ushort)MessageID.ReceiveChunk)]
        public static void ReceiveChunk(Message message)
        {
            Vector2 position = message.GetVector2();
            string dimension = message.GetString();
            byte[] compressedTiles = message.GetBytes();

            if (dimension != Main.Client.LocalPlayer.Dimension)
            {
                return;
            }

            byte[] decompressedTiles = Helper.Decompress(compressedTiles);

            ushort[] target = new ushort[decompressedTiles.Length / 2];

            Buffer.BlockCopy(decompressedTiles, 0, target, 0, decompressedTiles.Length);

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
