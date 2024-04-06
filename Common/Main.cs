using System;
using System.IO;
using System.Threading;
using Block2D.Modding;
using Block2D.Server;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoonSharp.Interpreter;
using NLog;
using NLog.Config;
using NLog.Targets;
using Steamworks;
using Version = System.Version;

namespace Block2D.Common
{
    public class Main : Game
    {
        public static InternalServer InternalServer
        {
            get => _instance._internalServer;
        }

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

        public static AssetManager AssetManager
        {
            get => _instance._assetManager;
        }

        public static bool OfflineMode { get; private set; }

        public static Random Random { get; private set; }

        public static Client.Client Client { get; private set; }

        public static bool ShouldExit { get; set; }

        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static Script Lua { get; private set; }

        public static KeyboardState KeyboardState { get; private set; }

        public static KeyboardState LastKeyboardState { get; private set; }

        public static Version Version { get; private set; }

        public const string GameName = "Block2D";

        private SpriteBatch _spriteBatch;
        private GraphicsDeviceManager _graphics;
        private readonly AssetManager _assetManager;
        private readonly InternalServer _internalServer;
        private readonly Thread _internalServerThread;
        private static Main _instance;
        private bool _windowSizeBeingChanged;

        public Main()
        {
            _instance = this;
            _internalServer = new();
            _internalServerThread = new(InternalServer.Run);
            _graphics = new(this);
            Random = new(0);
            Version = new(0, 1);
            _assetManager = new(Content);
            _windowSizeBeingChanged = false;
            Client = new();
            Content.RootDirectory = "Content";
            OfflineMode = false;
            IsMouseVisible = true;
            Lua = new();
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            InitializeLogger();

            Client.InitializeCamera(Window, GraphicsDevice);

            SetupLua();

            if (!SteamAPI.Init())
            {
                OfflineMode = true;
                Logger.Warn("(CLIENT): Failed To Connect To Steam.");
            }
            else
            {
                Logger.Info("(CLIENT): Successfully Connected To Steam.");
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            AssetManager.LoadContent();

            Client.LoadContent(this, _spriteBatch);

            Window.ClientSizeChanged += OnClientSizeChanged();
        }

        protected override void Update(GameTime gameTime)
        {
            LastKeyboardState = KeyboardState;
            KeyboardState = Keyboard.GetState();

            HandleGenericInput();

            Client.Tick(gameTime);

            if (ShouldExit)
            {
                Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: Client.Camera.GetViewMatrix());

            Client.Draw(_spriteBatch, AssetManager);

            _spriteBatch.End();

            //Client.UI.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            Client.Disconnect();

            if (_internalServer.IsRunning)
            {
                _internalServer.Stop();
            }

            base.OnExiting(sender, args);
        }

        private EventHandler<EventArgs> OnClientSizeChanged()
        {
            _windowSizeBeingChanged = !_windowSizeBeingChanged;

            if (_windowSizeBeingChanged)
            {
                _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                _graphics.ApplyChanges();
            }

            return null;
        }

        private void HandleGenericInput()
        {
            if (
                GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || KeyboardState.IsKeyDown(Keys.Escape)
            )
                ShouldExit = true;

            if (KeyboardState.IsKeyDown(Keys.G) && LastKeyboardState.IsKeyUp(Keys.G))
            {
                _internalServerThread.Start();
            }

            if (KeyboardState.IsKeyDown(Keys.F11) && LastKeyboardState.IsKeyUp(Keys.F11))
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

            if (KeyboardState.IsKeyDown(Keys.H))
            {
                Client.LocalConnect();
            }
        }

        public static void ForceQuitModloader()
        {
            _instance._assetManager.ForceQuit();
        }

        private void RegisterTypes()
        {
            UserData.RegisterType<KeyboardState>();
            UserData.RegisterType<Logger>();
            UserData.RegisterType<ModWorld>();
            UserData.RegisterType<ServerTile>();
        }

        public static void SetupScript(Script script)
        {
            DynValue keyboardState = UserData.Create(KeyboardState);
            DynValue lastKeyboardState = UserData.Create(LastKeyboardState);
            DynValue logger = UserData.Create(Logger);
            DynValue modWorld = UserData.Create(new ModWorld());

            script.Globals.Set("keyboardState", keyboardState);
            script.Globals.Set("lastKeyboardState", lastKeyboardState);
            script.Globals.Set("logger", logger);
            script.Globals.Set("world", modWorld);
        }

        private void SetupLua()
        {
            RegisterTypes();
            SetupScript(Lua);
        }

        public static GraphicsDevice GetGraphicsDevice() => _instance.GraphicsDevice;

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
    }
}
