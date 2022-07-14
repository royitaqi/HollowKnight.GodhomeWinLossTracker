using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;
using UnityEngine;
using Vasi;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class TKDeathDetector: Handler
    {
        public void OnGameLoaded(GameLoaded msg)
        {
            _logger.LogMod("Hooking FSM event: TK dream death");

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
                    _logger.LogMod("OnHeroDeathAnimStartInDream");
                    _bus.Put(new TKDreamDeath());
                });

            _logger.LogMod("Hooked FSM event: TK dream death");
        }
    }
}
