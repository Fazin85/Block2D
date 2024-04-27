using Block2D.Client.World;
using Block2D.Common;
using Microsoft.Xna.Framework;
using Riptide;

namespace Block2D.Client.Networking
{
    public class ClientMessageHandler
    {
        private readonly Client _client;

        public ClientMessageHandler(Client client)
        {
            _client = client;
        }

        public void TextSubmitted(string text)
        {
            Message message = Message.Create(MessageSendMode.Reliable, ClientMessageID.SendChatMessage);
            message.AddString(text);
            _client.Send(message);
        }

        public void PlayerJoin()
        {
            Message message = Message.Create(MessageSendMode.Reliable, ClientMessageID.PlayerJoin);
            message.AddString(_client.Username);
            _client.Send(message);
        }

        public void RequestChunk(Point position)
        {
            Message message = Message.Create(
                MessageSendMode.Unreliable,
                ClientMessageID.SendChunkRequest
            );
            message.AddPoint(position);
            message.AddString(_client.LocalPlayer.Dimension);
            _client.Send(message);
        }

        public void SendPosition(Vector2 position)
        {
            Message message = Message.Create(
                MessageSendMode.Unreliable,
                ClientMessageID.SendPosition
            );
            message.AddVector2(position);
            _client.Send(message);
        }

        public void ReceivePosition(Message message)
        {
            Vector2 position = message.GetVector2();
            ushort id = message.GetUShort();

            if (_client.CurrentWorld.Players.TryGetValue(id, out ClientPlayer player))
            {
                if (id == _client.ID)
                {
                    return;
                }

                player.Position = position;
            }
        }

        public void HandlePlayerSpawn(Message message)
        {
            string name = message.GetString();
            Vector2 position = message.GetVector2();
            ushort id = message.GetUShort();

            ClientPlayer newPlayer = new(_client, id, name) { Position = position, };

            _client.CurrentWorld.AddPlayer(newPlayer);

            if (_client.ID == id)
            {
                _client.OnJoinWorld();
                RequestChunk(Point.Zero);
                RequestChunk(new(64, 0));
                RequestChunk(new(64, -64));
                RequestChunk(new(0, -64));
            }
        }

        public void ReceiveChunk(Message message)
        {
            Point position = message.GetPoint();
            string dimension = message.GetString();
            byte offset = message.GetByte();
            byte[] compressedTiles = message.GetBytes();

            if (dimension != _client.LocalPlayer.Dimension)
            {
                return;
            }

            byte[] decompressedTileBytes = compressedTiles.Decompress();

            ushort[] decompressedTiles = decompressedTileBytes.ToUShortArray();

            if (offset == 0)
            {
                ClientChunk newChunk = new(position, _client);

                newChunk.SetSection(decompressedTiles, offset);
                _client.CurrentWorld.TryAddChunk(newChunk);
            }
            else
            {
                if (_client.CurrentWorld.GetChunkLoaded(position, out ClientChunk chunk))
                {
                    if (offset == chunk.ReceivedSections + 1 && offset < 4)
                    {
                        chunk.SetSection(decompressedTiles, offset);
                    }
                    else
                    {
                        _client.LogWarning("Received Corrupted Chunk Data.");
                    }
                }
            }
        }
    }
}
