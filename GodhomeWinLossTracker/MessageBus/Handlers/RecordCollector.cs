using GodhomeWinLossTracker.MessageBus.Messages;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class RecordCollector : Handler
    {
        public RecordCollector(IGodhomeWinLossTracker mod)
        {
            _mod = mod;
        }

        public void OnRawWinLoss(TheMessageBus bus, Modding.ILogger logger, RawWinLoss msg)
        {
            _mod.folderData.RawWinLosses.Add(msg);

            // Put the message back as registered. This should allow display a UI notification in the game.
            bus.Put(new RegisteredRawWinLoss { InnerMessage = msg });
        }

        public void OnRawHit(TheMessageBus bus, Modding.ILogger logger, RawHit msg)
        {
            _mod.folderData.RawHits.Add(msg);
        }

        private readonly IGodhomeWinLossTracker _mod;
    }
}
