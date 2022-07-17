namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class BossDeath : IMessage
    {
        public override string ToString()
        {
            return "Boss died";
        }
    }
}
