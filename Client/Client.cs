using Block2D.Client.Networking;
using Block2D.Common;
using Block2D.Modding;
using Block2D.Modding.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Block2D.Client
{
    public class Client : World
    {
        public ClientWorld World
        {
            get => _currentWorld ?? null;
        }

        public ClientPlayer LocalPlayer
        {
            get => World.GetPlayerFromId(_client.Id);
        }

        public ushort ID
        {
            get => _client.Id;
        }

        public bool DebugMode { get; set; }

        public bool InWorld { get; set; }

        public OrthographicCamera Camera { get; private set; }

        private readonly Riptide.Client _client;
        private ClientWorld _currentWorld;
        private const string ip = "127.0.0.1";
        private const ushort port = 7777;

        private ushort _nextTileIdToLoad;
        private readonly FPSCounter _fpsCounter;
        private const Keys DEBUG_KEY = Keys.F3;

        public Client()
        {
            LoadedTiles = new();
            _fpsCounter = new();
            _client = new();
            _client.Connected += OnConnect;
            _client.Disconnected += OnDisconnect;
            DebugMode = false;
        }

        public void InitializeCamera(GameWindow window, GraphicsDevice graphicsDevice)
        {
            BoxingViewportAdapter viewportAdapter = new(window, graphicsDevice, 800, 480);
            Camera = new OrthographicCamera(viewportAdapter);
        }

        private void OnConnect(object sender, EventArgs e)
        {
            _currentWorld = new();
            _nextTileIdToLoad = 0;
            _fpsCounter.Reset();
            ClientMessageHandler.PlayerJoin();
        }

        private void OnDisconnect(object sender, EventArgs e)
        {
            InWorld = false;
            _fpsCounter.Reset();
            _currentWorld = null;
        }

        public void Tick(GameTime gameTime)
        {
            if (
                Main.KeyboardState.IsKeyDown(DEBUG_KEY) && Main.LastKeyboardState.IsKeyUp(DEBUG_KEY)
            )
            {
                _fpsCounter.Reset();
                DebugMode = !DebugMode;
            }

            _client.Update();

            if (InWorld)
            {
                for (int i = 0; i < _currentWorld.Players.Count; i++)
                {
                    ClientPlayer currentPlayer = _currentWorld.Players.Values.ElementAt(i);
                    currentPlayer.Tick();
                }

                Camera.LookAt(LocalPlayer.Position);
            }

            if (DebugMode)
            {
                _fpsCounter.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch, AssetManager assets)
        {
            if (InWorld)
            {
                Renderer.DrawChunks(_currentWorld.Chunks.Values.ToArray(), spriteBatch);

                for (int i = 0; i < _currentWorld.Players.Count; i++)
                {
                    ClientPlayer currentPlayer = _currentWorld.Players.Values.ElementAt(i);
                    Renderer.DrawPlayer(currentPlayer, spriteBatch, assets);
                }
            }

            if (DebugMode)
            {
                _fpsCounter.Draw(spriteBatch, assets.Font, Camera.Position, Color.White);
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
            if (_client.IsConnecting || _client.IsConnected)
            {
                return;
            }

            _client.Connect($"{ip}:{port}");
        }

        public void Send(Message message)
        {
            _client.Send(message);
        }

        public void LoadContent() { }

        public override void LoadAllTiles()
        {
            for (int i = 0; i < Main.ModLoader.LoadedModCount; i++)
            {
                Mod currentMod = Main.ModLoader.LoadedMods.ElementAt(i);
                ModTile[] tiles = currentMod.ContentManager.GetModTiles();

                LoadModTiles(tiles);
            }
        }

        protected override void LoadModTiles(ModTile[] modTiles)
        {
            for (int i = 0; i < modTiles.Length; i++)
            {
                _nextTileIdToLoad++;
                LoadedTiles.Add(modTiles[i].Name, _nextTileIdToLoad);
            }
        }
    }
}
