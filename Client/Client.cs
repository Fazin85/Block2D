using Block2D.Client.Networking;
using Block2D.Common;
using Microsoft.Xna.Framework.Graphics;
using Riptide;
using System;

namespace Block2D.Client
{
    public class Client : ITickable
    {
        public ClientWorld World
        {
            get => _currentWorld == null ? null : _currentWorld;
        }

        public ClientPlayer LocalPlayer
        {
            get => World.GetPlayerFromId(_client.Id);
        }

        private Riptide.Client _client;
        private ClientWorld _currentWorld;
        private ClientMessageHandler _messageHandler;
        private bool _inWorld;
        private const string ip = "127.0.0.1";
        private const ushort port = 7777;

        public Client()
        {
            _messageHandler = new();
            _client = new();
            _client.Connected += OnConnect;
            _client.Disconnected += OnDisconnect;
        }

        private void OnConnect(object sender, EventArgs e)
        {
            _inWorld = true;
            _currentWorld = new();
            ClientPlayer lp = new(_client.Id);
            _currentWorld.AddPlayer(lp);
            _messageHandler.PlayerJoin();
        }

        private void OnDisconnect(object sender, EventArgs e)
        {
            _inWorld = false;
            _currentWorld = null;
        }

        public void Tick()
        {
            _client.Update();
        }

        public void Draw(SpriteBatch spriteBatch, AssetManager assets)
        {
            if (_inWorld)
            {
                Renderer.DrawChunks(_currentWorld.Chunks, spriteBatch, assets);
            }
        }

        public void Connect(string ip, ushort port)
        {
            if (_client.IsConnecting || _client.IsConnected)
            {
                return;
            }

            _client.Connect($"{ip}:{port}");
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }

        public void LocalConnect()
        {
            _client.Connect($"{ip}:{port}");
        }

        public void Send(Message message)
        {
            _client.Send(message);
        }
    }
}
