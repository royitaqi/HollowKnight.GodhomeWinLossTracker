using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    [ModLogLevel(Modding.LogLevel.Debug)]
    internal class BossHpPosRequest : IMessage
    {
        public override string ToString()
        {
            return "Request for BossHpPos";
        }
    }
}
