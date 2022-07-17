namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class RawHitRequest : IMessage
    {
        public override string ToString()
        {
            return "Request for RawHit";
        }
    }
}
