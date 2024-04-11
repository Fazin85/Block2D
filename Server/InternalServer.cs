using Block2D.Common;
using Riptide;
using System;
using System.Threading;

namespace Block2D.Server
{
    public class InternalServer
    {
        public static bool IsRunning
        {
            get => _instance._server != null && _instance._server.IsRunning;
        }

        public static ServerWorld World { get; private set; }

        public ServerState State { get; private set; }

        private readonly ServerLogger _logger;

        private Riptide.Server _server;
        private static Thread _executionThread;
        private static InternalServer _instance;

        private InternalServer()
        {
            State = ServerState.Inactive;
            _instance = this;
            _logger = new();
        }

        public static void Start()
        {
            if (_instance != null)
            {
                return;
            }

            _instance = new();

            _executionThread = new(_instance.Run);
            _executionThread.Start();
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

            _server.Start(7777, maxClientCount);
            _server.ClientConnected += OnClientConnected;
            _server.ClientDisconnected += OnClientDisconnected;

            World = new("DevWorld");

            World.LoadContent();

            State = ServerState.Loaded;
        }

        private void OnClientConnected(object sender, ServerConnectedEventArgs args) { }

        private void OnClientDisconnected(object sender, ServerDisconnectedEventArgs args)
        {
            World.RemovePlayer(args.Client.Id);
        }

        public static void Send(Message message, ushort toClientId)
        {
            _instance._server.Send(message, toClientId);
        }

        public static void SendToAll(Message message)
        {
            _instance._server.SendToAll(message);
        }

        public static void Stop()
        {
            _instance._server.Stop();
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
