using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    [ModLogLevel(Modding.LogLevel.Debug)]
    internal class RawHitRequest : IMessage
    {
        public override string ToString()
        {
            return "Request for RawHit";
        }
    }
}
