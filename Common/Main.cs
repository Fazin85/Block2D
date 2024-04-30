using Block2D.Modding;
using Block2D.Server;
using Block2D.Server.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        public static string AppDataDirectory
        {
            get => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        public static string GameAppDataDirectory
        {
            get => AppDataDirectory + "/" + GameName;
        }

        public static string ModsDirectory
        {
            get => GameAppDataDirectory + "/Mods";
        }

        public static string WorldsDirectory
        {
            get => GameAppDataDirectory + "/Worlds";
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
        private InternalServer _internalServer;
        private static Main _instance;
        private bool _windowSizeBeingChanged;

        #endregion

        public Main()
        {
            _instance = this;
            _graphics = new(this);
            Random = new(0);
            Client = new(Window, Content);
            _internalServer = new();
            _windowSizeBeingChanged = false;
            Content.RootDirectory = "Content";
            OfflineMode = false;
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
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

            Client.Initialize(this, Window, GraphicsDevice);

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

            Window.ClientSizeChanged += (o, s) => OnClientSizeChanged();
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

            base.OnExiting(sender, args);
        }

        #endregion

        #region private methods

        private void OnClientSizeChanged()
        {
            _windowSizeBeingChanged = !_windowSizeBeingChanged;

            if (_windowSizeBeingChanged)
            {
                _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                _graphics.ApplyChanges();
            }
        }

        private void HandleGenericInput()
        {
            if (
                GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || _keyboardState.IsKeyDown(Keys.Escape)
            )
                ShouldExit = true;

            if (_keyboardState.IsKeyDown(Keys.F11) && _lastKeyboardState.IsKeyUp(Keys.F11))
            {
                if (_graphics.IsFullScreen)
                {
                    _graphics.PreferredBackBufferWidth = 800;
                    _graphics.PreferredBackBufferHeight = 480;
                }
                else
                {
                    _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
                    _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
                }

                _graphics.IsFullScreen = !_graphics.IsFullScreen;
                _graphics.ApplyChanges();
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
