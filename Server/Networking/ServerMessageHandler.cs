using Microsoft.Xna.Framework;
using Riptide;

namespace Block2D.Server.Networking
{
    public class ServerMessageHandler
    {
        [MessageHandler((ushort)MessageID.HandlePlayerJoin)]
        public static void HandlePlayerJoin(ushort fromClientId, Message message)
        {
            string clientPlayerName = message.GetString();

            ServerPlayer newPlayer = new(Vector2.Zero, 20, clientPlayerName);

            InternalServer.Instance.World.AddPlayer(newPlayer);
        }
    }
}
