using Block2D.Client.Networking;
using Block2D.Common;
using Block2D.Common.ID;
using Block2D.Modding;
using Block2D.Modding.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Block2D.Client
{
    public class Client : World, ITickable
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

        public bool InWorld { get; set; }

        public OrthographicCamera Camera { get; private set; }

        private readonly Riptide.Client _client;
        private ClientWorld _currentWorld;
        private const string ip = "127.0.0.1";
        private const ushort port = 7777;

        public Dictionary<string, ushort> LoadedTiles;

        private ushort _nextTileIdToLoad;

        public Client()
        {
            LoadedTiles = new();
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
            _currentWorld = new();
            ClientMessageHandler.PlayerJoin();
        }

        private void OnDisconnect(object sender, EventArgs e)
        {
            InWorld = false;
            _currentWorld = null;
        }

        public void Tick()
        {
            _client.Update();

            if (InWorld)
            {
                for (int i = 0; i < _currentWorld.Players.Count; i++)
                {
                    ClientPlayer currentPlayer = _currentWorld.Players.Values.ElementAt(i);
                    currentPlayer.Tick();
                }

                Camera.LookAt(LocalPlayer.Position);

                //Camera.Position = LocalPlayer.Position;
            }
        }

        public void Draw(SpriteBatch spriteBatch, AssetManager assets)
        {
            if (InWorld)
            {
                Renderer.DrawChunks(_currentWorld.Chunks.Values.ToArray(), spriteBatch, assets);

                for (int i = 0; i < _currentWorld.Players.Count; i++)
                {
                    ClientPlayer currentPlayer = _currentWorld.Players.Values.ElementAt(i);
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
            LoadDefaultTiles();

            for (int i = 0; i < Main.ModLoader.LoadedModCount; i++)
            {
                Mod currentMod = Main.ModLoader.LoadedMods.ElementAt(i);
                ModTile[] tiles = currentMod.ContentManager.GetModTiles();

                LoadModTiles(tiles);
            }
        }

        protected override void LoadDefaultTiles()
        {
            LoadedTiles.Add(BlockID.AIR, 0);
            LoadedTiles.Add(BlockID.STONE, 1);
            LoadedTiles.Add(BlockID.DIRT, 2);
            LoadedTiles.Add(BlockID.GRASS, 3);
            _nextTileIdToLoad += 4;
        }

        protected override void LoadModTiles(ModTile[] modTiles)
        {
            for (int i = 0; i < modTiles.Length; i++)
            {
                _nextTileIdToLoad++;
                LoadedTiles.Add(modTiles[i].Name, _nextTileIdToLoad);
            }
        }

        public string GetTileName(ushort id)
        {
            var reversed = LoadedTiles.ToDictionary(x => x.Value, x => x.Key);
            return reversed[id];
        }
    }
}
