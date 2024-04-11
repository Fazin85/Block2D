using Block2D.Common;
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

        public ServerWorld World
        {
            get => _world;
        }

        private readonly ServerLogger _logger;

        public ServerState State { get; private set; }

        private Riptide.Server _server;
        private ServerWorld _world;
        private static InternalServer _instance;

        public InternalServer()
        {
            State = ServerState.Inactive;
            _instance = this;
            _logger = new();
        }

        public void Run()
        {
            Setup(20);

            while (!Main.ShouldExit)
            {
                if (_server.IsRunning)
                {
                    //do ticking stuff here
                    _world.Tick();

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

            _server.Start(7777, maxClientCount);
            _server.ClientConnected += OnClientConnected;
            _server.ClientDisconnected += OnClientDisconnected;

            _world = new("DevWorld");

            _world.LoadContent();

            State = ServerState.Loaded;
        }

        private void OnClientConnected(object sender, ServerConnectedEventArgs args) { }

        private void OnClientDisconnected(object sender, ServerDisconnectedEventArgs args)
        {
            _world.RemovePlayer(args.Client.Id);
        }

        public void Send(Message message, ushort toClientId)
        {
            _server.Send(message, toClientId);
        }

        public void SendToAll(Message message)
        {
            _server.SendToAll(message);
        }

        public void Stop()
        {
            _server.Stop();
        }

        public static void LogInfo(string message)
        {
            _instance._logger.LogInfo(message);
        }

        public static void LogWarning(string message)
        {
            _instance._logger.LogWarning(message);
        }

        public static void LogWarning(Exception exception)
        {
            _instance._logger.LogWarning(exception.Message);
        }

        public static void LogFatal(string message)
        {
            _instance._logger.LogFatal(message);
        }
    }
}
