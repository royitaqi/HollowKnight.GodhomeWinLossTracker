using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;
using SFCore.Utils;
using UnityEngine;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class TKDeathAndStatusObserver: Handler
    {
        // Shouldn't unload, because we need OnSceneChange() to get "Menu_Title" scene event and so we can clear _hooked.
        public override void Unload()
        {
            // Don't unload
        }

        public void OnSceneChange(SceneChange msg)
        {
            // Clear _hooked, because the hero game object has been killed.
            if (msg.Name == "Menu_Title")
            {
                _hooked = false;
            }
            // For other scenes (assuming in game), hook if haven't
            else if (!_hooked)
            {
                _logger.LogModDebug("Hooking TK's FSM events");

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
                _logger.LogModDebug("Hooked TK's FSM event: Fsm_OnHeroDeathAnimStart");

                // Hook TK early damage event
                hero.LocateMyFSM("ProxyFSM")
                    .GetState("Damaged")
                    .InsertMethod(Fsm_OnKnightDamaged, 0);
                _logger.LogModDebug("Hooked TK's FSM event: Fsm_OnKnightDamaged");

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
