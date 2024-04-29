using Block2D.Client.Networking;
using Block2D.Client.UI;
using Block2D.Client.World;
using Block2D.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using Riptide;
using Steamworks;
using System;
using RectangleF = MonoGame.Extended.RectangleF;

namespace Block2D.Client
{
    public class Client
    {
        #region public variables

        public const int MAX_USERNAME_LENGTH = 16;

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

        public bool Connected
        {
            get => _client.IsConnected;
        }

        public bool DebugMode { get; set; }
        public string Username { get; private set; }

        public OrthographicCamera Camera { get; private set; }

        public DebugMenu DebugMenu { get; private set; }

        public bool InWorld { get; private set; }

        public ClientState State { get; private set; }

        public ClientMessageHandler MessageHandler { get; private set; }

        public Chat Chat { get; private set; }

        public ClientLogger Logger { get; private set; }

        public PlayerListUI PlayerListUI { get; private set; }

        public EventHandler OnJoinWorld;

        public bool InMainMenu
        {
            get => State == ClientState.MainMenu;
        }

        #endregion

        #region private variables

        private bool _canConnect
        {
            get => !_client.IsConnected && !_client.IsConnecting && !InWorld;
        }

        private ClientWorld _currentWorld;
        private MainMenu _mainMenu;
        private readonly Riptide.Client _client;
        private readonly WorldRenderer _worldRenderer;
        private const string ip = "127.0.0.1";
        private const ushort port = 7777;
        private const Keys DEBUG_KEY = Keys.F3;
        private byte _uiUpdateTime;

        #endregion

        public Client(GameWindow window, ContentManager contentManager)
        {
            AssetManager = new(contentManager);
            DebugMenu = new();
            _worldRenderer = new(this);
            MessageHandler = new(this);
            Chat = new(window);
            Chat.TextSubmitted += MessageHandler.TextSubmitted;
            Logger = new();
            PlayerListUI = new(this);
            _client = new();
            _client.MessageReceived += OnMessageReceived;
            _client.Connected += OnConnect;
            _client.Disconnected += OnDisconnect;
            DebugMode = false;
            OnJoinWorld += OnEnterWorld;
        }

        #region public methods

        //do all client initializing here
        public void Initialize(Main main, GameWindow window, GraphicsDevice graphicsDevice)
        {
            State = ClientState.Initializing;
            BoxingViewportAdapter viewportAdapter = new(window, graphicsDevice, 800, 480);
            Camera = new OrthographicCamera(viewportAdapter);

            _mainMenu = new(main);
        }

        //do all client content loading here
        public void LoadContent()
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

            State = ClientState.MainMenu;
        }

        public void Update(
            KeyboardState keyboard,
            KeyboardState lastKeyboardState,
            GameTime gameTime
        )
        {
            _client.Update();

            if (InMainMenu)
            {
                return;
            }

            _uiUpdateTime++;

            if (keyboard.IsKeyDown(DEBUG_KEY) && lastKeyboardState.IsKeyUp(DEBUG_KEY))
            {
                DebugMenu.Reset();
                DebugMode = !DebugMode;
            }

            if (InWorld && !Chat.IsOpen)
            {
                _currentWorld.Tick(gameTime);

                Camera.LookAt(LocalPlayer.Position);

                if (_uiUpdateTime == 60)
                {
                    PlayerListUI.Update();
                    _uiUpdateTime = 0;
                }
            }

            if (DebugMode)
            {
                DebugMenu.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch, KeyboardState keyboard)
        {
            if (InMainMenu)
            {
                _mainMenu.Draw(spriteBatch, AssetManager);
            }
            else
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

                    foreach (ClientPlayer currentPlayer in _currentWorld.Players.Values)
                    {
                        currentPlayer.Draw(spriteBatch, AssetManager);
                    }
                }

                Vector2 chatDrawPosition = new(Camera.Position.X, Camera.Position.Y + Camera.BoundingRectangle.Height - 28);

                Chat.Draw(spriteBatch, AssetManager, chatDrawPosition, Camera.BoundingRectangle);

                PlayerListUI.Draw(spriteBatch, Camera.Position, Main.GraphicsDevice.PresentationParameters.BackBufferWidth / 2, keyboard);

                if (DebugMode)
                {
                    DebugMenu.Draw(spriteBatch, AssetManager.Font, Camera.Position, Color.White);
                }

                spriteBatch.End();
            }
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
            if (InMainMenu)
            {
                return;
            }

            MessageHandler.SendDisconnect();

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

        public void InvokeJoinWorld()
        {
            OnJoinWorld.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region private methods

        private void OnEnterWorld(object sender, EventArgs e)
        {
            InWorld = true;
        }

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
                    MessageHandler.ReceivePosition(e.Message, e.FromConnection.SmoothRTT);
                    break;
                case (ushort)ClientMessageID.HandlePlayerSpawn:
                    MessageHandler.HandlePlayerSpawn(e.Message, e.FromConnection.SmoothRTT);
                    break;
                case (ushort)ClientMessageID.ReceiveChunk:
                    MessageHandler.ReceiveChunk(e.Message);
                    break;
                case (ushort)ClientMessageID.ReceiveDisconnect:
                    MessageHandler.HandlePlayerDisconnect(e.Message);
                    break;
            }
        }

        #endregion
    }
}
