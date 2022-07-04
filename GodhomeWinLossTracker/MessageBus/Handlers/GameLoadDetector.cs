using GodhomeWinLossTracker.MessageBus.Messages;
using Vasi;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class GameLoadDetector : Handler
    {
        public new void OnMessage(TheMessageBus bus, Modding.ILogger logger, IMessage message)
        {
            if (message is LoadFolderData)
            {
                _freshlyLoaded = true;
            }
            else if (message is SceneChange)
            {
                // Trigger game loaded event by first scene change
                if (_freshlyLoaded)
                {
                    _freshlyLoaded = false;
                    bus.Put(new GameLoaded());
                }
            }
        }

        private bool _freshlyLoaded = false;
    }
}
