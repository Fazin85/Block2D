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
    public class ClientMain : World
    {
        public static ClientWorld World
        {
            get => _instance._currentWorld ?? null;
        }

        public ClientState State { get; private set; }

        public static ClientPlayer LocalPlayer
        {
            get => World.GetPlayerFromId(_instance._client.Id);
        }

        public static ushort ID
        {
            get => _instance._client.Id;
        }

        public static bool DebugMode { get; set; }

        public bool InWorld { get; private set; }

        public static string Username { get; private set; }

        public static OrthographicCamera Camera { get; private set; }

        public UiSystem UI { get; private set; }

        public static DebugMenu DebugMenu { get; private set; }

        private bool _canConnect
        {
            get => !_client.IsConnected && !_client.IsConnecting && !InWorld;
        }

        private readonly Riptide.Client _client;
        private ClientWorld _currentWorld;
        private const string ip = "127.0.0.1";
        private const ushort port = 7777;
        private const Keys DEBUG_KEY = Keys.F3;
        private static ClientMain _instance;

        public ClientMain()
        {
            LoadedTiles = new();
            DebugMenu = new();
            _client = new();
            _client.Connected += OnConnect;
            _client.Disconnected += OnDisconnect;
            DebugMode = false;
            NextTileIdToLoad = 0;
        }

        //do all client initializing here
        public static void Initialize(GameWindow window, GraphicsDevice graphicsDevice)
        {
            if (_instance == null)
            {
                _instance = new();
            }

            _instance.State = ClientState.Initializing;
            BoxingViewportAdapter viewportAdapter = new(window, graphicsDevice, 800, 480);
            Camera = new OrthographicCamera(viewportAdapter);
        }

        //do all client content loading here
        public static void LoadContent(Game game, SpriteBatch spriteBatch)
        {
            _instance.State = ClientState.Loading;

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

            _instance.UI = new UiSystem(game, style);
            var panel = new Panel(Anchor.Center, size: new(100, 100), positionOffset: Vector2.Zero);
            _instance.UI.Add("Panel", panel);

            //load tiles
            _instance.LoadTiles();
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

        public static void OnJoinWorld()
        {
            _instance.InWorld = true;
        }

        public static void Tick(GameTime gameTime)
        {
            if (
                Main.KeyboardState.IsKeyDown(DEBUG_KEY) && Main.LastKeyboardState.IsKeyUp(DEBUG_KEY)
            )
            {
                DebugMenu.Reset();
                DebugMode = !DebugMode;
            }

            _instance._client.Update();

            if (_instance.InWorld)
            {
                _instance._currentWorld.Tick(gameTime);

                Camera.LookAt(LocalPlayer.Position);
            }

            _instance.UI.Update(gameTime);

            if (DebugMode)
            {
                DebugMenu.Update(gameTime);
            }
        }

        public static void Draw(SpriteBatch spriteBatch, AssetManager assets)
        {
            if (_instance.InWorld)
            {
                RectangleF viewRect = Camera.BoundingRectangle;
                viewRect.Inflate(CC.TILE_SIZE, CC.TILE_SIZE);

                Renderer.DrawChunks(_instance._currentWorld.Chunks.Values.ToArray(), spriteBatch, viewRect);

                for (int i = 0; i < _instance._currentWorld.Players.Count; i++)
                {
                    ClientPlayer currentPlayer = _instance._currentWorld.Players.Values.ElementAt(i);
                    Renderer.DrawPlayer(currentPlayer, spriteBatch, assets);
                }
            }

            if (DebugMode)
            {
                DebugMenu.Draw(spriteBatch, assets.Font, Camera.Position, Color.White);
            }
        }

        public static void Connect(string ip, ushort port)
        {
            if (!_instance._canConnect)
            {
                return;
            }

            _instance._client.Connect($"{ip}:{port}");
            _instance.State = ClientState.Multiplayer;
        }

        public static void Disconnect()
        {
            _instance._client.Disconnect();
        }

        public static void LocalConnect()
        {
            if (!_instance._canConnect)
            {
                return;
            }

            _instance._client.Connect($"{ip}:{port}");
            _instance.State = ClientState.Singleplayer;
        }

        public static string GetTileName(ushort id)
        {
            return _instance.GEtTileName(id);
        }

        public static void Send(Message message)
        {
            _instance._client.Send(message);
        }

        public static ClientMain GetInstance()
        {
            return _instance;
        }
    }
}
