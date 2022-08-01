using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;
using UnityEngine;
using Vasi;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class TKDeathAndStatusObserver: Handler
    {
        public void OnSceneChange(SceneChange _)
        {
            if (!_hooked)
            {
                _logger.LogMod("Hooking TK's FSM events");

                // This FSM event detects TK dream death.
                GameObject hero = HeroController.instance?.gameObject;
                // GameObject hero = GameObject.Find("Knight"); // Another way of getting the hero

                if (hero == null)
                {
                    return;
                }

                // Hook TK death event
                hero.transform
                    .Find("Hero Death").gameObject
                    .LocateMyFSM("Hero Death Anim")
                    .GetState("Anim Start") // For TK real death, use "Map Zone" instead of "Anim Start".
                    .AddMethod(Fsm_OnHeroDeathAnimStart);
                _logger.LogMod("Hooked TK's FSM event: Fsm_OnHeroDeathAnimStart");

                // Hook TK early damage event
                hero.LocateMyFSM("ProxyFSM")
                    .GetState("Damaged")
                    .InsertMethod(0, Fsm_OnKnightDamaged);
                _logger.LogMod("Hooked TK's FSM event: Fsm_OnKnightDamaged");

                _hooked = true;
            }
        }

        public void Fsm_OnHeroDeathAnimStart()
        {
            if (_loaded)
            {
                _logger.LogModDebug("On FSM event: GO=Knight/Hero Death FSM=Hero Death Anim State=Anim Start");
                _bus.Put(new TKDreamDeath());
            }
        }

        public void Fsm_OnKnightDamaged()
        {
            if (_loaded)
            {
                _logger.LogModDebug("On FSM event: GO=Knight FSM=ProxyFSM State=Damaged");
                _bus.Put(new TKStatus { Status = TKUtils.GetTKStatus() });
            }
        }

        private bool _hooked = false;
    }
}
