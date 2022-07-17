using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class RegisteredRawWinLoss : IMessage
    {
        public RawWinLoss InnerMessage { get; set; }

        public override string ToString()
        {
            DevUtils.Assert(InnerMessage != null, "InnerMessage shouldn't be null");
            return $"{InnerMessage} (registered)";
        }
    }
}
