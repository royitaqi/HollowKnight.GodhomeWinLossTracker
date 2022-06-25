using GodhomeWinLossTracker.MessageBus.Messages;
using UnityEngine;
using Vasi;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class TKDeathDetector: IHandler
    {
        public TKDeathDetector()
        {
        }

        public void OnMessage(TheMessageBus bus, Modding.ILogger logger, IMessage msg)
        {
            // Clear flag when loading a new game save.
            // Need to hook for every loads because game objects are newly created at load and they don't carry the previously hooked methods.
            // Cannot hook at the loading time, because the necessary game objects have not been initialized yet (e.g. TK).
            if (msg is LoadFolderData)
            {
                fsmHooked = false;
            }
            // Set up hook at the first scene change
            else if (msg is SceneChange)
            {
                // Only hook once for each load
                if (!fsmHooked)
                {
#if DEBUG
                    logger.Log("Hooking FSM event: TK dream death");
#endif
                    _bus = bus;
                    _logger = logger;
                    // This FSM event detects TK dream death.
                    // For TK real death, use "Map Zone" instead of "Anim Start".
                    GameObject
                        .Find("Knight").transform
                        .Find("Hero Death").gameObject
                        .LocateMyFSM("Hero Death Anim")
                        .GetState("Anim Start")
                        .AddMethod(OnHeroDeathAnimStartInDream);
#if DEBUG
                    logger.Log("Hooked FSM event: TK dream death");
#endif
                    fsmHooked = true;
                }
            }
        }

        private void OnHeroDeathAnimStartInDream()
        {
#if DEBUG
            _logger.Log("OnHeroDeathAnimStartInDream");
#endif
            _bus.Put(new TKDreamDeath());
        }

        private bool fsmHooked = false;
        private TheMessageBus _bus;
        private Modding.ILogger _logger;
    }
}
