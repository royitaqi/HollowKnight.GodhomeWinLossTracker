using GodhomeWinLossTracker.MessageBus.Messages;
using UnityEngine;
using Vasi;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class TKHitDetector: IHandler
    {
        public void OnMessage(TheMessageBus bus, Modding.ILogger logger, IMessage msg)
        {
            if (msg is GameLoaded)
            {
#if DEBUG
                logger.Log("Hooking FSM event: TK hit");
#endif

                GameObject hero = HeroController.instance.gameObject;
                // GameObject hero = GameObject.Find("Knight"); // Another way of getting the hero

                PlayMakerFSM fsm = hero
                    .LocateMyFSM("ProxyFSM");

                fsm.GetState("Flower?")
                    .InsertMethod(0, () =>
                    {
#if DEBUG
                        logger.Log($"TK took a hit: {PlayerData.instance.health + PlayerData.instance.healthBlue}");
#endif
                    });

#if DEBUG
                logger.Log("Hooked FSM event: TK hit");
#endif
            }
        }
    }
}
