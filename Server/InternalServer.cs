using Block2D.Common;
using Block2D.Server.Networking;
using Riptide;
using System;
using System.Threading;

namespace Block2D.Server
{
    public class InternalServer
    {
        public bool IsRunning
        {
            get => _server != null && _server.IsRunning;
        }

        public ServerLogger Logger { get; private set; }

        public ServerWorld World { get; private set; }

        public ServerState State { get; private set; }

        public ServerMessageHandler MessageHandler { get; private set; }

        public ServerAssetManager AssetManager { get; private set; }

        private readonly ServerLogger _logger;
        private Riptide.Server _server;
        private Thread _executionThread;

        public InternalServer()
        {
            State = ServerState.Inactive;
            MessageHandler = new(this);
            _logger = new();
            AssetManager = new();
        }

        public void Start()
        {
            _executionThread = new(Run);
            _executionThread.Start();
        }

        public void Exit()
        {
            if (IsRunning)
            {
                Stop();
            }
        }

        public void Send(Message message, ushort toClientId)
        {
            _server.Send(message, toClientId);
        }

        public void SendToAll(Message message)
        {
            _server.SendToAll(message);
        }

        private void Stop()
        {
            _server.Stop();

            AssetManager.UnloadAllMods();
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

        private void Run()
        {
            Setup(20);

            while (!Main.ShouldExit)
            {
                if (_server.IsRunning)
                {
                    //do ticking stuff here
                    World.Update();

                    _server.Update();
                }

                Thread.Sleep(16);
            }
        }

        private void Setup(ushort maxClientCount)
        {
            if (State == ServerState.Starting)
            {
                return;
            }

            State = ServerState.Starting;

            if (IsRunning)
            {
                return;
            }

            _server = new Riptide.Server();

            _server.MessageReceived += OnMessageReceived;
            _server.ClientConnected += OnClientConnected;
            _server.ClientDisconnected += OnClientDisconnected;

            _server.Start(7777, maxClientCount, useMessageHandlers: false);

            AssetManager.LoadContent();

            World = new(AssetManager, this, "DevWorld");

            World.LoadContent();

            State = ServerState.Loaded;
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            switch (e.MessageId)
            {
                case (ushort)ServerMessageID.HandlePlayerJoin:
                    MessageHandler.HandlePlayerJoin(e.FromConnection.Id, e.Message);
                    break;
                case (ushort)ServerMessageID.ReceivePosition:
                    MessageHandler.ReceivePosition(e.FromConnection.Id, e.Message);
                    break;
                case (ushort)ServerMessageID.HandleChunkRequest:
                    MessageHandler.HandleChunkRequest(e.FromConnection.Id, e.Message);
                    break;
            }
        }

        private void OnClientConnected(object sender, ServerConnectedEventArgs args) { }

        private void OnClientDisconnected(object sender, ServerDisconnectedEventArgs args)
        {
            World.RemovePlayer(args.Client.Id);
        }
    }
}
