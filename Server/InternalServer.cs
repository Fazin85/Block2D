using Riptide.Utils;

namespace Block2D.Server
{
    public class InternalServer : ITickable
    {
        public bool IsRunning
        {
            get => _server.IsRunning;
        }

        public World World
        {
            get => _world;
        }

        private Riptide.Server _server;
        private World _world;

        public void Start(ushort maxClientCount)
        {
            RiptideLogger.Initialize(Main.Logger.Info, false);

            _server = new Riptide.Server();
            _server.Start(7777, maxClientCount);
        }

        public void Tick()
        {
            _server.Update();
        }

        public void Stop()
        {
            _server.Stop();
        }
    }
}
