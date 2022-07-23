using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    [ModLogLevel(Modding.LogLevel.Debug)]
    internal class TKHpPos : IMessage
    {
        public int HP { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public override string ToString()
        {
            return $"TK HP={HP} Pos=({X}, {Y})";
        }
    }
}
