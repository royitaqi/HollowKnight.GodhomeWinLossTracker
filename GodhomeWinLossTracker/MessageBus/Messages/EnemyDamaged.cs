using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    // Currently this message is being generated and put onto the bus, but not processed.
    [ModLogLevel(Modding.LogLevel.Debug)]
    internal class EnemyDamaged : IMessage
    {
        public override string ToString()
        {
            return $"Enemy damaged";
        }
    }
}
