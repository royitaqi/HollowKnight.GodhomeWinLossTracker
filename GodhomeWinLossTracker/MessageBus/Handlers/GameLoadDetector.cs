using GodhomeWinLossTracker.MessageBus.Messages;
using Vasi;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class GameLoadDetector : IHandler
    {
        public void OnMessage(TheMessageBus bus, Modding.ILogger logger, IMessage message)
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

#if DEBUG
                    HeroController.instance.transform.Find("Effects/Damage Effect").gameObject.LocateMyFSM("Knight Damage").GetState("Gen").InsertMethod(0, () =>
                    {
                        logger.Log($"TK took dmg. HP = {PlayerData.instance.health + PlayerData.instance.healthBlue}");
                    });
#endif
                }
            }
        }

        private bool _freshlyLoaded = false;
    }
}
