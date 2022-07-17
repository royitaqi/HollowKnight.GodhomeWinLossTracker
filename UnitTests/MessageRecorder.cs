namespace UnitTests
{
    // Validates that some of the messages received match the given ones in order.
    internal class MessageRecorder : Handler
    {
        public List<IMessage> Messages { get; set; } = new();

        public override void OnMessage(IMessage msg)
        {
            Messages.Add(msg);
        }
    }
}