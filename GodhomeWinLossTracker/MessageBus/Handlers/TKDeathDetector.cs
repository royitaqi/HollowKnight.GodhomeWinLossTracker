using GodhomeWinLossTracker.MessageBus.Messages;
using UnityEngine;
using Vasi;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class TKDeathDetector: Handler
    {
        public new void OnMessage(TheMessageBus bus, Modding.ILogger logger, IMessage msg)
        {
            if (msg is GameLoaded)
            {
#if DEBUG
                logger.Log("Hooking FSM event: TK dream death");
#endif

                // This FSM event detects TK dream death.
                // For TK real death, use "Map Zone" instead of "Anim Start".
                GameObject hero = HeroController.instance.gameObject;
                // GameObject hero = GameObject.Find("Knight"); // Another way of getting the hero

                PlayMakerFSM fsm = hero.transform
                    .Find("Hero Death").gameObject
                    .LocateMyFSM("Hero Death Anim");

                fsm.GetState("Anim Start")
                    .AddMethod(() =>
                    {
#if DEBUG
                        logger.Log("OnHeroDeathAnimStartInDream");
#endif
                        bus.Put(new TKDreamDeath());
                    });

#if DEBUG
                logger.Log("Hooked FSM event: TK dream death");
#endif
            }
        }
    }
}
