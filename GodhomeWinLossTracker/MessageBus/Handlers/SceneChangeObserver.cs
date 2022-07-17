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
        }

        private string ModHooks_BeforeSceneLoadHook(string sceneName)
        {
            _mod.LogMod($"OnSceneLoad: {sceneName}");
            _bus.Put(new SceneChange { Name = sceneName });
            return sceneName;
        }
    }
}
