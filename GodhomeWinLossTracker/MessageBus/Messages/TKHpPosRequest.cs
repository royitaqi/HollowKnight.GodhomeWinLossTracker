using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    [ModLogLevel(Modding.LogLevel.Debug)]
    internal class TKHpPosRequest : IMessage
    {
        public override string ToString()
        {
            return "Request for TKHpPos";
        }
    }
}
