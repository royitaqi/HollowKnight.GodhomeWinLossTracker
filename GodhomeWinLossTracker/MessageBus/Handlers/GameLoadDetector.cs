using GodhomeWinLossTracker.MessageBus.Messages;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class GameLoadDetector : Handler
    {
        public void OnLoadFolderData(TheMessageBus bus, Modding.ILogger logger, LoadFolderData msg)
        {
            _freshlyLoaded = true;
        }

        public void OnSceneChange(TheMessageBus bus, Modding.ILogger logger, SceneChange msg)
        {
            // Trigger game loaded event by first scene change
            if (_freshlyLoaded)
            {
                _freshlyLoaded = false;
                bus.Put(new GameLoaded());
            }
        }

        private bool _freshlyLoaded = false;
    }
}
