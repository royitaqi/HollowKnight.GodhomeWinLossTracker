namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class Logger : Handler
    {
        public new void OnMessage(TheMessageBus bus, Modding.ILogger logger, IMessage msg)
        {
            logger.Log($"Message on bus: {msg}");
        }
    }
}
