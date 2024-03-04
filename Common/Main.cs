using System;
using System.IO;
using Block2D.Server;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Block2D.Common
{
    public class Main : Game
    {
        public static Client.Client Client { get; private set; }

        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static string AppDataDirectory
        {
            get => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        public const string GameName = "Block2D";

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private readonly AssetManager _assetManager;
        private readonly InternalServer _internalServer;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            _assetManager = new(Content);
            _internalServer = new();
            Client = new();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            InitializeLogger();
            Client.InitializeCamera(Window, GraphicsDevice);
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _assetManager.LoadBlockTextures();

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (
                GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape)
            )
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.G))
            {
                _internalServer.Start(20);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.H))
            {
                Client.LocalConnect();
            }

            Client.Tick();

            if (_internalServer.IsRunning)
            {
                _internalServer.Tick();
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: Client.Camera.GetViewMatrix());

            Client.Draw(_spriteBatch, _assetManager);

            _spriteBatch.End();
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
