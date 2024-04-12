﻿using Block2D.Common;
using Microsoft.Xna.Framework;
using Riptide;

namespace Block2D.Server.Networking
{
    public class ServerMessageHandler
    {
        private readonly InternalServer _server;

        public ServerMessageHandler(InternalServer server)
        {
            _server = server;
        }

        public void HandlePlayerJoin(ushort fromClientId, Message message)
        {
            string clientPlayerName = message.GetString();

            ServerPlayer newPlayer = new(fromClientId, -Vector2.UnitY * 16, 20, clientPlayerName);

            foreach (ServerPlayer otherPlayer in _server.World.Players.Values)
            {
                _server.Send(CreateSpawnMessage(otherPlayer), fromClientId);
            }

            _server.World.AddPlayer(newPlayer);
            _server.SendToAll(CreateSpawnMessage(newPlayer));
        }

        public static Message CreateSpawnMessage(ServerPlayer player)
        {
            Message message = Message.Create(MessageSendMode.Reliable, ServerMessageID.PlayerSpawn);
            message.AddString(player.Name);
            message.AddVector2(player.Position);
            message.AddUShort(player.ID);
            return message;
        }

        public void SendPositions()
        {
            foreach (ServerPlayer player in _server.World.Players.Values)
            {
                Message message = Message.Create(
                    MessageSendMode.Unreliable,
                    ServerMessageID.SendPosition
                );
                message.AddVector2(player.Position);
                message.AddUShort(player.ID);
                _server.SendToAll(message);
            }
        }

        public void ReceivePosition(ushort fromClientId, Message message)
        {
            Vector2 position = message.GetVector2();
            if (_server.World.Players.TryGetValue(fromClientId, out ServerPlayer player))
            {
                player.Position = position;
            }
        }

        public void HandleChunkRequest(ushort fromClientId, Message message)
        {
            Point position = message.GetPoint();
            string playerDimension = message.GetString();

            if (
                _server
                    .World.Dimensions[playerDimension]
                    .ChunkManager.GetOrTryAddChunk(position, playerDimension, out ServerChunk chunk)
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

                    byte[] tileBytes = chunk.Sections[i].Tiles.ToByteArray();
                    byte[] bytesToSend = tileBytes.Compress();

                    newMessage.AddBytes(bytesToSend);
                    _server.Send(newMessage, fromClientId);
                }
            }
        }
    }
}
