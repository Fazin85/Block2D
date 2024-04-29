using Block2D.Client.UI;
using Block2D.Client.World;
using Block2D.Common;
using Microsoft.Xna.Framework;
using Riptide;
using Steamworks;

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
            message.AddULong(Main.OfflineMode ? 0 : SteamUser.GetSteamID().m_SteamID);
            message.AddBool(Main.OfflineMode);
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

        public void SendDisconnect()
        {
            Message message = Message.Create(MessageSendMode.Reliable, ClientMessageID.SendDisconnect);
            _client.Send(message);
        }

        public void ReceivePosition(Message message, short ping)
        {
            Vector2 position = message.GetVector2();
            ushort id = message.GetUShort();

            if (_client.CurrentWorld.Players.TryGetValue(id, out ClientPlayer player))
            {
                if (id == _client.ID)
                {
                    return;
                }

                player.Ping = ping;
                player.Position = position;
            }
        }

        public void HandlePlayerDisconnect(Message message)
        {
            ushort id = message.GetUShort();

            if (id == _client.ID)
            {
                return;
            }

            _client.PlayerListUI.RemovePlayer(id);
            _client.CurrentWorld.RemovePlayer(id);
        }

        public void HandlePlayerSpawn(Message message, short connectionPing)
        {
            string name = message.GetString();
            Vector2 position = message.GetVector2();
            ushort id = message.GetUShort();
            ulong steamID = message.GetULong();
            bool offlineMode = message.GetBool();

            ClientPlayer newPlayer = new(_client, id, name, offlineMode, steamID)
            {
                Position = position
            };

            _client.CurrentWorld.AddPlayer(newPlayer);

            PlayerListEntry entry = new()
            {
                SteamID = steamID,
                Name = name,
                Ping = connectionPing
            };

            _client.PlayerListUI.AddPlayer(entry);

            _client.PlayerListUI.Update();

            if (_client.ID == id)
            {
                _client.InvokeJoinWorld();

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
                        _client.Logger.LogWarning("Received Corrupted Chunk Data.");
                    }
                }
            }
        }
    }
}
