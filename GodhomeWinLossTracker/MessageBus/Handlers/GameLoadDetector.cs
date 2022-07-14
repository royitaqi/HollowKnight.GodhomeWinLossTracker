using GodhomeWinLossTracker.MessageBus.Messages;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class GameLoadDetector : Handler
    {
        public void OnLoadFolderData(LoadFolderData msg)
        {
            _freshlyLoaded = true;
        }

        public void OnSceneChange(SceneChange msg)
        {
            // Trigger game loaded event by first scene change
            if (_freshlyLoaded)
            {
                _freshlyLoaded = false;
                _bus.Put(new GameLoaded());
            }
        }

        private bool _freshlyLoaded = false;
    }
}
