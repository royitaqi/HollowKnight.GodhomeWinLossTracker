using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    [ModLogLevel(Modding.LogLevel.Debug)]
    public class TKStatus: IMessage
    {
        public int Status { get; set; }

        public override string ToString()
        {
            return $"TK status: {Status}";
        }
    }
}
