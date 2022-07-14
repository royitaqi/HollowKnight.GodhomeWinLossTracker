using GodhomeWinLossTracker.MessageBus.Messages;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class RecordCollector : Handler
    {
        public void OnRawWinLoss(RawWinLoss msg)
        {
            _mod.folderData.RawWinLosses.Add(msg);

            // Put the message back as registered. This should allow display a UI notification in the game.
            _bus.Put(new RegisteredRawWinLoss { InnerMessage = msg });
        }

        public void OnRawHit(RawHit msg)
        {
            _mod.folderData.RawHits.Add(msg);
        }
    }
}
