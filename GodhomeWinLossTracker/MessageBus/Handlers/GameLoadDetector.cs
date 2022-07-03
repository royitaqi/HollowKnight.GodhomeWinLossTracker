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
                    var trans = HeroController.instance.transform.Find("Hero Death");
                    if (trans == null)
                    {
                        logger.Log("trans null");
                    }
                    var trans2 = HeroController.instance.transform.Find("Knight Damage Effect");
                    if (trans2 == null)
                    {
                        logger.Log("trans2 null");
                    }
                    //var trans2 = trans.gameObject;
                    //if (trans2 == null)
                    //{
                    //    logger.Log("trans2 null");
                    //}
                    //var fsm = trans2.LocateMyFSM("Hero Death Anim");
                    //fsm.GetState("Gen").AddMethod(() =>
                    //{
                    //    logger.Log($"TK took hit. Health = {PlayerData.instance.health + PlayerData.instance.healthBlue}");
                    //});
#endif
                }
            }
        }

        private bool _freshlyLoaded = false;
    }
}
