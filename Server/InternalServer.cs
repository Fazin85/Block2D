using Block2D.Common;
using Riptide;
using Riptide.Utils;

namespace Block2D.Server
{
    public class InternalServer : ITickable
    {
        public bool IsRunning
        {
            get => _server != null && _server.IsRunning;
        }

        public ServerWorld World
        {
            get => _world;
        }

        private Riptide.Server _server;
        private readonly ServerWorld _world;

        public InternalServer()
        {
            _world = new("DevWorld");
        }

        public void Start(ushort maxClientCount)
        {
            if (IsRunning)
            {
                return;
            }

            _server = new Riptide.Server();

            RiptideLogger.Initialize(Main.Logger.Info, false);

            _server.Start(7777, maxClientCount);
            _server.ClientConnected += OnClientConnected;
        }

        public void OnClientConnected(object sender, ServerConnectedEventArgs args)
        {

        }

        public void OnClientDisconnected(object sender, ServerDisconnectedEventArgs args)
        {
            _world.RemovePlayer(args.Client.Id);
        }

        public void Tick()
        {
            _world.Tick();

            _server.Update();
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
