using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;
using UnityEngine;
using Vasi;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class TKDeathAndStatusObserver: Handler
    {
        public void OnGameLoaded(GameLoaded _)
        {
            _logger.LogMod("Hooking FSM event: TK dream death");

            // This FSM event detects TK dream death.
            GameObject hero = HeroController.instance.gameObject;
            // GameObject hero = GameObject.Find("Knight"); // Another way of getting the hero

            // Hook TK death event
            hero.transform
                .Find("Hero Death").gameObject
                .LocateMyFSM("Hero Death Anim")
                .GetState("Anim Start") // For TK real death, use "Map Zone" instead of "Anim Start".
                .AddMethod(() =>
                {
                    _logger.LogModDebug("On FSM event: GO=Knight/Hero Death FSM=Hero Death Anim State=Anim Start");
                    _bus.Put(new TKDreamDeath());
                });
            _logger.LogMod("Hooked FSM event: TK dream death");

            // Hook TK early damage event
            hero.LocateMyFSM("ProxyFSM")
                .GetState("Damaged")
                .InsertMethod(0, () =>
                {
                    _logger.LogModDebug("On FSM event: GO=Knight FSM=ProxyFSM State=Damaged");
                    _bus.Put(new TKStatus { Status = TKUtils.GetTKStatus() });
                });
            _logger.LogMod("Hooked FSM event: TK damaged");
        }
    }
}
