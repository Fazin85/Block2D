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
using System;
using System.Linq;
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

        public OrthographicCamera Camera { get; private set; }

        public UiSystem UI { get; private set; }

        public DebugMenu DebugMenu { get; private set; }

        private readonly Riptide.Client _client;
        private ClientWorld _currentWorld;
        private const string ip = "127.0.0.1";
        private const ushort port = 7777;

        private ushort _nextTileIdToLoad;
        private const Keys DEBUG_KEY = Keys.F3;
        private long _tickCounter;

        public Client()
        {
            LoadedTiles = new();
            DebugMenu = new();
            _client = new();
            _client.Connected += OnConnect;
            _client.Disconnected += OnDisconnect;
            DebugMode = false;
            _tickCounter = 0;
        }
        //do all client content loading here
        public void LoadContent(Game game, SpriteBatch spriteBatch)
        {
            State = ClientState.Loading;
            MlemPlatform.Current = new MlemPlatform.DesktopGl<TextInputEventArgs>((w, c) => w.TextInput += c);

            var style = new UntexturedStyle(spriteBatch)
            {
                Font = new GenericSpriteFont(Main.AssetManager.Font)
            };

            UI = new UiSystem(game, style);
            var panel = new Panel(Anchor.Center, size: new(100, 100), positionOffset: Vector2.Zero);
            UI.Add("Panel", panel);

            //load tiles
            LoadAllTiles();
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
            _nextTileIdToLoad = 0;
            DebugMenu.Reset();
            ClientMessageHandler.PlayerJoin();
            _tickCounter = 0;
        }

        private void OnDisconnect(object sender, EventArgs e)
        {
            State = ClientState.MainMenu;
            InWorld = false;
            DebugMenu.Reset();
            _currentWorld = null;
            _tickCounter = 0;
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
                for (int i = 0; i < _currentWorld.Players.Count; i++)
                {
                    ClientPlayer currentPlayer = _currentWorld.Players.Values.ElementAt(i);
                    currentPlayer.Tick(gameTime);

                    if(_tickCounter % 3 == 0)
                    {
                        ClientMessageHandler.SendPosition(currentPlayer.Position);
                    }
                }

                Camera.LookAt(LocalPlayer.Position);
            }

            UI.Update(gameTime);

            if (DebugMode)
            {
                DebugMenu.Update(gameTime);
            }

            _tickCounter++;
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

        protected override void LoadAllTiles()
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
                LoadedTiles.Add(modTiles[i].Name, _nextTileIdToLoad);
                _nextTileIdToLoad++;
            }
        }
    }
}
