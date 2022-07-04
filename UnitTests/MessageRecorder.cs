using Modding;

namespace UnitTests
{
    // Validates that some of the messages received match the given ones in order.
    internal class MessageRecorder : Handler
    {
        public List<IMessage> Messages { get; set; } = new();

        public void OnMessage(TheMessageBus bus, Modding.ILogger logger, IMessage msg)
        {
            Messages.Add(msg);
        }
    }
}