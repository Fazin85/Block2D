using Block2D.Client.Networking;
using Block2D.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using Riptide;
using System;
using System.Linq;

namespace Block2D.Client
{
    public class Client : ITickable
    {
        public ClientWorld World
        {
            get => _currentWorld ?? null;
        }

        public ClientPlayer LocalPlayer
        {
            get => World.GetPlayerFromId(_client.Id);
        }

        public OrthographicCamera Camera { get; private set; }

        private readonly Riptide.Client _client;
        private ClientWorld _currentWorld;
        private readonly ClientMessageHandler _messageHandler;
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

        public void InitializeCamera(GameWindow window, GraphicsDevice graphicsDevice)
        {
            BoxingViewportAdapter viewportAdapter = new(window, graphicsDevice, 800, 480);
            Camera = new OrthographicCamera(viewportAdapter);
        }

        private void OnConnect(object sender, EventArgs e)
        {
            _inWorld = true;
            _currentWorld = new();
            ClientPlayer lp = new(_client.Id);
            lp.Position = -Vector2.UnitY * 16;
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

            if (_inWorld)
            {
                for (int i = 0; i < _currentWorld.Players.Count; i++)
                {
                    ClientPlayer currentPlayer = _currentWorld.Players[i];
                    currentPlayer.Tick();
                }

                Camera.LookAt(LocalPlayer.Position);

                //Camera.Position = LocalPlayer.Position;
            }
        }

        public void Draw(SpriteBatch spriteBatch, AssetManager assets)
        {
            if (_inWorld)
            {
                Renderer.DrawChunks(_currentWorld.Chunks.Values.ToArray(), spriteBatch, assets);

                for (int i = 0; i < _currentWorld.Players.Count; i++)
                {
                    ClientPlayer currentPlayer = _currentWorld.Players[i];
                    Renderer.DrawPlayer(currentPlayer, spriteBatch, assets);
                }
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
