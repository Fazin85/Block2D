using Block2D.Server.Commands;
using Microsoft.Xna.Framework;

namespace Block2D.Server
{
    public class ServerPlayer
    {
        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public Vector2 Velocity
        {
            get => _velocity;
            set => _velocity = value;
        }

        public int Health
        {
            get => _health;
            set => _health = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public ushort ID
        {
            get => _id;
        }

        public ulong SteamID { get; private set; }

        public bool OfflineMode { get; private set; }

        public PermissionLevel PermissionLevel { get; set; }

        private Vector2 _position;
        private Vector2 _velocity;
        private int _health;
        private string _name;
        private readonly ushort _id;

        public ServerPlayer(ushort id, Vector2 position, int health, string name, PermissionLevel permissions, ulong steamID, bool offlineMode)
        {
            _id = id;
            _position = position;
            _health = health;
            _name = name;
            PermissionLevel = permissions;
            SteamID = steamID;
            OfflineMode = offlineMode;
        }

        public void Tick()
        {
        }
    }
}
