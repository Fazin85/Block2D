using Block2D.Client.Networking;
using Block2D.Common;
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
using System;
using System.Linq;
using RectangleF = MonoGame.Extended.RectangleF;

namespace Block2D.Client
{
    public class ClientMain : WorldData
    {
        public static ClientWorld CurrentWorld
        {
            get => _instance._currentWorld ?? null;
        }

        public static ClientPlayer LocalPlayer
        {
            get => CurrentWorld.GetPlayerFromId(_instance._client.Id);
        }

        public static ushort ID
        {
            get => _instance._client.Id;
        }

        private readonly ClientLogger _logger;

        public static bool DebugMode { get; set; }
        public static string Username { get; private set; }

        public static OrthographicCamera Camera { get; private set; }

        public UiSystem UI { get; private set; }

        public DebugMenu DebugMenu { get; private set; }

        public bool InWorld { get; private set; }

        public ClientState State { get; private set; }

        private bool _canConnect
        {
            get => !_client.IsConnected && !_client.IsConnecting && !InWorld;
        }

        private ClientWorld _currentWorld;
        private readonly Riptide.Client _client;
        private readonly WorldRenderer _worldRenderer;
        private const string ip = "127.0.0.1";
        private const ushort port = 7777;
        private const Keys DEBUG_KEY = Keys.F3;
        private static ClientMain _instance;

        public ClientMain()
        {
            _instance = this;
            DebugMenu = new();
            _worldRenderer = new();
            _logger = new();
            _client = new();
            _client.Connected += OnConnect;
            _client.Disconnected += OnDisconnect;
            DebugMode = false;
        }

        //do all client initializing here
        public void Initialize(GameWindow window, GraphicsDevice graphicsDevice)
        {
            State = ClientState.Initializing;
            BoxingViewportAdapter viewportAdapter = new(window, graphicsDevice, 800, 480);
            Camera = new OrthographicCamera(viewportAdapter);
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

        public void OnJoinWorld()
        {
            InWorld = true;
        }

        public void Update(GameTime gameTime)
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

                _worldRenderer.DrawChunks(_currentWorld.Chunks.Values.ToArray(), spriteBatch, viewRect);

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
            if (!_canConnect)
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
            if (!_canConnect)
            {
                return;
            }

            _client.Connect($"{ip}:{port}");
            State = ClientState.Singleplayer;
        }

        public string GetTileName(ushort id)
        {
            return GEtTileName(id);
        }

        public static void Send(Message message)
        {
            _instance._client.Send(message);
        }

        public void LogInfo(string message)
        {
            _logger.LogInfo(message);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }

        public void LogWarning(Exception exception)
        {
            _logger.LogWarning(exception.Message);
        }

        public void LogFatal(string message)
        {
            _logger.LogFatal(message);
        }

        public static ClientMain GetInstance()
        {
            return _instance;
        }
    }
}
