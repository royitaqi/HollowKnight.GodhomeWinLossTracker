using GodhomeWinLossTracker.MessageBus.Messages;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class WinLossTracker : Handler
    {
        public WinLossTracker(IGodhomeWinLossTracker mod)
        {
            _mod = mod;
        }

        public void OnRawWinLoss(TheMessageBus bus, Modding.ILogger logger, RawWinLoss msg)
        {
            _mod.folderData.RawRecords.Add(msg);

            // Put the message back as registered. This should allow display a UI notification in the game.
            bus.Put(new RegisteredRawWinLoss { InnerMessage = msg });
        }

        public void OnRawTKHit(TheMessageBus bus, Modding.ILogger logger, RawTKHit msg)
        {
            _mod.folderData.RawTKHits.Add(msg);
        }

        private readonly IGodhomeWinLossTracker _mod;
    }
}
