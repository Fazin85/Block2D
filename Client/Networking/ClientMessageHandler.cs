using Riptide;

namespace Block2D.Client.Networking
{
    public class ClientMessageHandler
    {
        public static void PlayerJoin()
        {
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.PlayerJoin);
            message.AddString("Hello World");
            Main.Client.Send(message);
        }
    }
}
