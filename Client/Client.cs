using Block2D.Client.Networking;
using Block2D.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
    public class Client
    {
        #region public variables

        public ClientWorld CurrentWorld
        {
            get => _currentWorld ?? null;
        }

        public ClientPlayer LocalPlayer
        {
            get => CurrentWorld.GetPlayerFromId(_client.Id);
        }

        public ClientAssetManager AssetManager { get; private set; }

        public ushort ID
        {
            get => _client.Id;
        }

        public bool DebugMode { get; set; }
        public string Username { get; private set; }

        public OrthographicCamera Camera { get; private set; }

        public UiSystem UI { get; private set; }

        public DebugMenu DebugMenu { get; private set; }

        public bool InWorld { get; private set; }

        public ClientState State { get; private set; }

        public ClientMessageHandler MessageHandler { get; private set; }

        public Chat Chat { get; private set; }

        #endregion

        #region private variables

        private bool _canConnect
        {
            get => !_client.IsConnected && !_client.IsConnecting && !InWorld;
        }

        private ClientWorld _currentWorld;
        private readonly ClientLogger _logger;
        private readonly Riptide.Client _client;
        private readonly WorldRenderer _worldRenderer;
        private const string ip = "127.0.0.1";
        private const ushort port = 7777;
        private const Keys DEBUG_KEY = Keys.F3;

        #endregion

        public Client(GameWindow window, ContentManager contentManager)
        {
            AssetManager = new(contentManager);
            DebugMenu = new();
            _worldRenderer = new(this);
            MessageHandler = new(this);
            Chat = new(window);
            Chat.TextSubmitted += MessageHandler.TextSubmitted;
            _logger = new();
            _client = new();
            _client.MessageReceived += OnMessageReceived;
            _client.Connected += OnConnect;
            _client.Disconnected += OnDisconnect;
            DebugMode = false;
        }

        #region public methods

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

            AssetManager.LoadContent();

            MlemPlatform.Current = new MlemPlatform.DesktopGl<TextInputEventArgs>(
                (w, c) => w.TextInput += c
            );

            var style = new UntexturedStyle(spriteBatch)
            {
                Font = new GenericSpriteFont(AssetManager.Font)
            };

            UI = new UiSystem(game, style);
            var panel = new Panel(Anchor.Center, size: new(100, 100), positionOffset: Vector2.Zero);
            UI.Add("Panel", panel);
        }

        public void OnJoinWorld()
        {
            InWorld = true;
        }

        public void Update(
            KeyboardState keyboard,
            KeyboardState lastKeyboardState,
            GameTime gameTime
        )
        {
            if (keyboard.IsKeyDown(DEBUG_KEY) && lastKeyboardState.IsKeyUp(DEBUG_KEY))
            {
                DebugMenu.Reset();
                DebugMode = !DebugMode;
            }

            _client.Update();

            if (InWorld && !Chat.IsOpen)
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

        public void Draw(SpriteBatch spriteBatch, GameWindow window)
        {
            spriteBatch.Begin(transformMatrix: Camera.GetViewMatrix());

            if (InWorld)
            {
                RectangleF viewRect = Camera.BoundingRectangle;
                viewRect.Inflate(CC.TILE_SIZE, CC.TILE_SIZE);

                _worldRenderer.DrawChunks(
                    [.. _currentWorld.Chunks.Values],
                    spriteBatch,
                    viewRect,
                    DebugMode
                );

                for (int i = 0; i < _currentWorld.Players.Count; i++)
                {
                    ClientPlayer currentPlayer = _currentWorld.Players.Values.ElementAt(i);
                    Renderer.DrawPlayer(currentPlayer, spriteBatch, AssetManager);
                }
            }

            Vector2 chatDrawPosition = new(Camera.Position.X, Camera.Position.Y + Camera.BoundingRectangle.Height - 28);

            Chat.Draw(spriteBatch, AssetManager, chatDrawPosition, Camera.BoundingRectangle);

            if (DebugMode)
            {
                DebugMenu.Draw(spriteBatch, AssetManager.Font, Camera.Position, Color.White);
            }

            spriteBatch.End();
        }

        public void Connect(string ip, ushort port)
        {
            if (!_canConnect)
            {
                return;
            }

            _client.Connect($"{ip}:{port}", useMessageHandlers: false);
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

            _client.Connect($"{ip}:{port}", useMessageHandlers: false);
            State = ClientState.Singleplayer;
        }

        public void Send(Message message)
        {
            _client.Send(message);
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

        #endregion

        #region private methods

        private void OnConnect(object sender, EventArgs e)
        {
            _currentWorld = new(AssetManager, this);
            DebugMenu.Reset();
            MessageHandler.PlayerJoin();
        }

        private void OnDisconnect(object sender, EventArgs e)
        {
            State = ClientState.MainMenu;
            InWorld = false;
            DebugMenu.Reset();
            _currentWorld = null;
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            switch (e.MessageId)
            {
                case (ushort)ClientMessageID.ReceivePosition:
                    MessageHandler.ReceivePosition(e.Message);
                    break;
                case (ushort)ClientMessageID.HandlePlayerSpawn:
                    MessageHandler.HandlePlayerSpawn(e.Message);
                    break;
                case (ushort)ClientMessageID.ReceiveChunk:
                    MessageHandler.ReceiveChunk(e.Message);
                    break;
            }
        }

        #endregion
    }
}
