using System;
using System.Linq;
using Block2D.Client.Networking;
using Block2D.Common;
using Block2D.Modding;
using Block2D.Modding.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MLEM.Font;
using MLEM.Misc;
using MLEM.Ui;
using MLEM.Ui.Elements;
using MLEM.Ui.Style;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using Riptide;
using Steamworks;
using RectangleF = MonoGame.Extended.RectangleF;

namespace Block2D.Client
{
    public class Client : World
    {
        public ClientWorld World
        {
            get => _currentWorld ?? null;
        }

        public ClientState State { get; private set; }

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

        public string Username { get; private set; }

        public OrthographicCamera Camera { get; private set; }

        public UiSystem UI { get; private set; }

        public DebugMenu DebugMenu { get; private set; }

        private readonly Riptide.Client _client;
        private ClientWorld _currentWorld;
        private const string ip = "127.0.0.1";
        private const ushort port = 7777;
        private const Keys DEBUG_KEY = Keys.F3;

        public Client()
        {
            LoadedTiles = new();
            DebugMenu = new();
            _client = new();
            _client.Connected += OnConnect;
            _client.Disconnected += OnDisconnect;
            DebugMode = false;
            NextTileIdToLoad = 0;
        }

        //do all client content loading here
        public void LoadContent(Game game, SpriteBatch spriteBatch)
        {
            State = ClientState.Loading;

            if (Main.OfflineMode)
            {
                Username = "Player" + Random.Shared.Next(1000).ToString();
            }
            else
            {
                Username = SteamFriends.GetPersonaName();
            }

            MlemPlatform.Current = new MlemPlatform.DesktopGl<TextInputEventArgs>(
                (w, c) => w.TextInput += c
            );

            var style = new UntexturedStyle(spriteBatch)
            {
                Font = new GenericSpriteFont(Main.AssetManager.Font)
            };

            UI = new UiSystem(game, style);
            var panel = new Panel(Anchor.Center, size: new(100, 100), positionOffset: Vector2.Zero);
            UI.Add("Panel", panel);

            //load tiles
            LoadTiles();
        }

        //do all client initializing here
        public void InitializeCamera(GameWindow window, GraphicsDevice graphicsDevice)
        {
            State = ClientState.Initializing;
            BoxingViewportAdapter viewportAdapter = new(window, graphicsDevice, 800, 480);
            Camera = new OrthographicCamera(viewportAdapter);
        }

        private void OnConnect(object sender, EventArgs e)
        {
            _currentWorld = new();
            DebugMenu.Reset();
            ClientMessageHandler.PlayerJoin();
        }

        private void OnDisconnect(object sender, EventArgs e)
        {
            State = ClientState.MainMenu;
            InWorld = false;
            DebugMenu.Reset();
            _currentWorld = null;
        }

        public void Tick(GameTime gameTime)
        {
            if (
                Main.KeyboardState.IsKeyDown(DEBUG_KEY) && Main.LastKeyboardState.IsKeyUp(DEBUG_KEY)
            )
            {
                DebugMenu.Reset();
                DebugMode = !DebugMode;
            }

            _client.Update();

            if (InWorld)
            {
                _currentWorld.Tick(gameTime);

                Camera.LookAt(LocalPlayer.Position);
            }

            UI.Update(gameTime);

            if (DebugMode)
            {
                DebugMenu.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch, AssetManager assets)
        {
            if (InWorld)
            {
                RectangleF viewRect = Camera.BoundingRectangle;
                viewRect.Inflate(CC.TILE_SIZE, CC.TILE_SIZE);

                Renderer.DrawChunks(_currentWorld.Chunks.Values.ToArray(), spriteBatch, viewRect);

                for (int i = 0; i < _currentWorld.Players.Count; i++)
                {
                    ClientPlayer currentPlayer = _currentWorld.Players.Values.ElementAt(i);
                    Renderer.DrawPlayer(currentPlayer, spriteBatch, assets);
                }
            }

            if (DebugMode)
            {
                DebugMenu.Draw(spriteBatch, assets.Font, Camera.Position, Color.White);
            }
        }

        public void Connect(string ip, ushort port)
        {
            if (_client.IsConnecting || _client.IsConnected)
            {
                return;
            }

            _client.Connect($"{ip}:{port}");
            State = ClientState.Multiplayer;
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
            State = ClientState.Singleplayer;
        }

        public void Send(Message message)
        {
            _client.Send(message);
        }
    }
}
