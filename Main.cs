﻿using System;
using System.IO;
using Block2D.Server;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NLog;
using NLog.Config;
using NLog.Targets;
using Riptide;

namespace Block2D
{
    public class Main : Game
    {
        public static Client.Client Client
        {
            get => _client;
        }

        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private readonly AssetManager _assetManager;
        private readonly InternalServer _internalServer;
        private static Client.Client _client;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            _assetManager = new(Content);
            _internalServer = new();
            _client = new();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            InitializeLogger();
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
                _client.LocalConnect();
            }

            _client.Tick();

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

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
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