using System.Threading;
using Block2D.Common;
using Riptide;
using Riptide.Utils;

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

        public ServerState State { get; private set; }

        private Riptide.Server _server;
        private ServerWorld _world;

        public InternalServer()
        {
            State = ServerState.Inactive;
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

            RiptideLogger.Initialize(Main.Logger.Info, false);

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
    }
}
