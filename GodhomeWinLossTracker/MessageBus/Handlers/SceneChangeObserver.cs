using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;
using Modding;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class SceneChangeObserver: Handler
    {
        public override void Load(IGodhomeWinLossTracker mod, TheMessageBus bus, Modding.ILogger logger)
        {
            base.Load(mod, bus, logger);
            ModHooks.BeforeSceneLoadHook += ModHooks_BeforeSceneLoadHook;
            _on = true;
        }

        public override void Unload()
        {
            // Don't unload
        }

        private string ModHooks_BeforeSceneLoadHook(string sceneName)
        {
            _mod.LogMod($"OnSceneLoad: {sceneName}");

            if (sceneName.StartsWith("GG_") && sceneName != "GG_Waterways")
            {
                // Load the bus if it isn't loaded
                if (!_on)
                {
                    _bus.Put(new BusCommand { Command = BusCommand.Commands.Load });
                    _on = true;
                }

                // Annouce scene change after bus is loaded
                _bus.Put(new SceneChange { Name = sceneName });
            }
            else
            {
                // Unload the bus if it is loaded
                if (_on)
                {
                    _bus.Put(new BusCommand { Command = BusCommand.Commands.Unload });
                    _on = false;
                }
            }

            return sceneName;
        }

        private bool _on = false;
    }
}
