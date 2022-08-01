namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class BusCommand : IMessage
    {
        public enum Commands
        {
            Load,
            Unload,
        }

        public Commands Command { get; set; }

        public override string ToString()
        {
            return $"Command bus to {Command}";
        }
    }
}
