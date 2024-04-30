using Block2D.Modding;
using Block2D.Server;
using Block2D.Server.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ViewportAdapters;
using MoonSharp.Interpreter;
using NLog;
using NLog.Config;
using NLog.Targets;
using Riptide.Utils;
using Steamworks;
using System;
using System.IO;

namespace Block2D.Common
{
    public class Main : Game
    {
        #region public variables

        public static string ModsDirectory
        {
            get => "Mods";
        }

        public static string WorldsDirectory
        {
            get => "Worlds";
        }

        public static new GraphicsDevice GraphicsDevice
        {
            get => _instance._game.GraphicsDevice;
        }

        public static bool OfflineMode { get; private set; }

        public static Random Random { get; private set; }

        public static bool ShouldExit { get; set; }

        public const string GameName = "Block2D";

        public Client.Client Client { get; private set; }

        #endregion

        #region private variables

        private Game _game
        {
            get => _instance;
        }

        private KeyboardState _keyboardState;
        private KeyboardState _lastKeyboardState;
        private SpriteBatch _spriteBatch;
        private readonly GraphicsDeviceManager _graphics;
        private readonly InternalServer _internalServer;
        private static Main _instance;
        private bool _fakeFullScreen;

        #endregion

        public Main()
        {
            _instance = this;
            _graphics = new(this);
            Random = new(0);
            Client = new(Window, Content);
            _internalServer = new();
            Content.RootDirectory = "Content";
            OfflineMode = false;
            IsMouseVisible = true;
            _fakeFullScreen = false;
        }

        #region public methods

        public void StartSinglePlayer()
        {
            _internalServer.Start();

            Client.LocalConnect();
        }

        public static void SetupScript(Script script, Mod mod, bool setupLogger, ProgramSide side)
        {
            DynValue keyboardState = UserData.Create(_instance._keyboardState);
            DynValue lastKeyboardState = UserData.Create(_instance._lastKeyboardState);

            script.Globals.Set("keyboardState", keyboardState);
            script.Globals.Set("lastKeyboardState", lastKeyboardState);

            if (setupLogger)
            {
                DynValue logger = UserData.Create(mod.Logger);
                script.Globals.Set("logger", logger);

                DynValue modWorld = null;

                if (side == ProgramSide.Client)
                {
                    modWorld = UserData.Create(new ModWorld(_instance.Client.CurrentWorld, mod));
                }
                else
                {
                    modWorld = UserData.Create(new ModWorld(_instance._internalServer.World, mod));
                }

                script.Globals.Set("world", modWorld);
            }
        }

        #endregion

        #region protected methods

        protected override void Initialize()
        {
            InitializeLogger();

            RiptideLogger.Initialize(DefaultLogger.Instance.RiptideLog, false);

            RegisterTypes();

            Client.Initialize(this, Window, GraphicsDevice, _graphics);

            if (!SteamAPI.Init())
            {
                OfflineMode = true;
                Client.Logger.LogWarning("Failed To Connect To Steam.");
            }
            else
            {
                Client.Logger.LogInfo("Successfully Connected To Steam.");
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new(GraphicsDevice);

            Client.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            _lastKeyboardState = _keyboardState;
            _keyboardState = Keyboard.GetState();

            HandleGenericInput();

            Client.Update(_keyboardState, _lastKeyboardState, gameTime);

            if (ShouldExit)
            {
                Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Client.Draw(_spriteBatch, _keyboardState);

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            Client.Disconnect();

            _internalServer?.Exit();

            Client.Settings.Save();

            base.OnExiting(sender, args);
        }

        #endregion

        #region private methods

        private void HandleGenericInput()
        {
            if (
                GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || _keyboardState.IsKeyDown(Keys.Escape)
            )
                ShouldExit = true;

            if (_keyboardState.IsKeyDown(Keys.F11) && _lastKeyboardState.IsKeyUp(Keys.F11))
            {
                if (_fakeFullScreen)
                {
                    _graphics.PreferredBackBufferWidth = Client.Settings.ResoloutionX;
                    _graphics.PreferredBackBufferHeight = Client.Settings.ResoloutionY;

                    Client.Settings.ResoloutionX = _graphics.PreferredBackBufferWidth;
                    Client.Settings.ResoloutionY = _graphics.PreferredBackBufferHeight;
                }
                else
                {
                    EnterFullscreen(false);
                }

                Client.Settings.FullScreen = _fakeFullScreen = !_fakeFullScreen;
                Client.Settings.Save();

                _graphics.ApplyChanges();

                BoxingViewportAdapter adapter = new(Window, GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
                Client.Camera = new(adapter);
            }
        }

        public static void EnterFullscreen(bool apply)
        {
            _instance._graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            _instance._graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;

            if (apply)
            {
                _instance.Client.Settings.FullScreen = _instance._fakeFullScreen = !_instance._fakeFullScreen;
                _instance.Client.Settings.Save();

                _instance._graphics.ApplyChanges();

                BoxingViewportAdapter adapter = new(_instance.Window, GraphicsDevice, _instance._graphics.PreferredBackBufferWidth, _instance._graphics.PreferredBackBufferHeight);
                _instance.Client.Camera = new(adapter);
            }
        }

        private void RegisterTypes()
        {
            UserData.RegisterType<KeyboardState>();
            UserData.RegisterType<ModLogger>();
            UserData.RegisterType<ModWorld>();
            UserData.RegisterType<ServerTile>();
        }

        private static void InitializeLogger()
        {
            string logFilePath = "Logs/ClientLog.txt";

            if (File.Exists(logFilePath)) //copying the existing log file to a new file and changing its name
            {
                FileInfo existingLogFileInfo = new(logFilePath);

                DateTime existingLogFileCreationTime = existingLogFileInfo.CreationTime;
                existingLogFileInfo.CopyTo(
                    "Logs/ClientLog"
                        + existingLogFileCreationTime.Year
                        + "-"
                        + existingLogFileCreationTime.Day
                        + "-"
                        + existingLogFileCreationTime.Hour
                        + "-"
                        + existingLogFileCreationTime.Minute
                        + "-"
                        + existingLogFileCreationTime.Second
                        + ".txt"
                );

                existingLogFileInfo.Delete();
            }

            LoggingConfiguration config = new();
            FileTarget target = new("Log") { FileName = logFilePath };
            config.AddRule(LogLevel.Info, LogLevel.Fatal, target);

            LogManager.Configuration = config;
        }

        #endregion
    }
}
