using Riptide;

namespace Block2D.Client
{
    public class Client : ITickable
    {
        public Riptide.Client BaseClient
        {
            get => _client;
        }

        private Riptide.Client _client;
        private World _currentWorld;
        private bool _inWorld;
        private const string ip = "127.0.0.1";
        private const ushort port = 7777;

        public Client()
        {
            _client = new();
            // _client.Connected += 
        }

        public void Tick()
        {
            _client.Update();
        }

        public void Connect(string ip, ushort port)
        {
            if (_client.IsConnecting || _client.IsConnected)
            {
                return;
            }

            _client.Connect($"{ip}:{port}");
        }

        public void LocalConnect()
        {
            _client.Connect($"{ip}:{port}");
        }

        public void Send(Message message)
        {
            _client.Send(message);
        }
    }
}
