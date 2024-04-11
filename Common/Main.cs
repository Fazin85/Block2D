using Block2D.Client;
using Block2D.Modding;
using Block2D.Server;
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

        public static new GraphicsDevice GraphicsDevice
        {
            get => _instance._game.GraphicsDevice;
        }

        public static bool OfflineMode { get; private set; }

        public static Random Random { get; private set; }

        public static bool ShouldExit { get; set; }

        public static KeyboardState KeyboardState { get; private set; }

        public static KeyboardState LastKeyboardState { get; private set; }

        public const string GameName = "Block2D";

        private Game _game
        {
            get => _instance;
        }

        private SpriteBatch _spriteBatch;
        private readonly GraphicsDeviceManager _graphics;
        private readonly AssetManager _assetManager;
        private static Main _instance;
        private bool _windowSizeBeingChanged;

        public Main()
        {
            _instance = this;
            _graphics = new(this);
            Random = new(0);
            _assetManager = new(Content);
            _windowSizeBeingChanged = false;
            Content.RootDirectory = "Content";
            OfflineMode = false;
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            InitializeLogger();

            RiptideLogger.Initialize(DefaultLogger.Instance.RiptideLog, false);

            RegisterTypes();

            ClientMain.Initialize(Window, GraphicsDevice);

            if (!SteamAPI.Init())
            {
                OfflineMode = true;
                ClientMain.LogWarning("Failed To Connect To Steam.");
            }
            else
            {
                ClientMain.LogInfo("Successfully Connected To Steam.");
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            AssetManager.LoadContent();

            ClientMain.LoadContent(this, _spriteBatch);

            Window.ClientSizeChanged += OnClientSizeChanged();
        }

        protected override void Update(GameTime gameTime)
        {
            LastKeyboardState = KeyboardState;
            KeyboardState = Keyboard.GetState();

            HandleGenericInput();

            ClientMain.Update(gameTime);

            if (ShouldExit)
            {
                Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: ClientMain.Camera.GetViewMatrix());

            ClientMain.Draw(_spriteBatch, AssetManager);

            _spriteBatch.End();

            //Client.UI.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            ClientMain.Disconnect();

            if (InternalServer.IsRunning)
            {
                InternalServer.Stop();
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
                InternalServer.Start();
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
                ClientMain.LocalConnect();
            }
        }

        public static void ForceQuitModloader()
        {
            _instance._assetManager.ForceQuit();
        }

        private void RegisterTypes()
        {
            UserData.RegisterType<KeyboardState>();
            UserData.RegisterType<ModLogger>();
            UserData.RegisterType<ModWorld>();
            UserData.RegisterType<ServerTile>();
        }

        public static void SetupScript(Script script, Mod mod, bool setupLogger)
        {
            DynValue keyboardState = UserData.Create(KeyboardState);
            DynValue lastKeyboardState = UserData.Create(LastKeyboardState);
            DynValue modWorld = UserData.Create(new ModWorld(mod));

            script.Globals.Set("keyboardState", keyboardState);
            script.Globals.Set("lastKeyboardState", lastKeyboardState);
            script.Globals.Set("world", modWorld);

            if (setupLogger)
            {
                DynValue logger = UserData.Create(mod.Logger);
                script.Globals.Set("logger", logger);
            }
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
    }
}
