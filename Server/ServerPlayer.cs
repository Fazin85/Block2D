using Block2D.Common;
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
        
        private Vector2 _position;
        private Vector2 _velocity;
        private int _health;
        private string _name;
        private ushort _id;

        public ServerPlayer(ushort id, Vector2 position, int health, string name)
        {
            _id = id;
            _position = position;
            _health = health;
            _name = name;
        }

        public void Tick()
        {
        }
    }
}
