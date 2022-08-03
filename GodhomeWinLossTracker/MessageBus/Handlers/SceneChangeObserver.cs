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
            ModHooks.SceneChanged += ModHooks_SceneChanged;
            On.GameManager.BeginScene += GameManager_BeginScene;
            On.GameManager.BeginSceneTransition += GameManager_BeginSceneTransition;
            _on = true;
        }

        private void GameManager_BeginSceneTransition(On.GameManager.orig_BeginSceneTransition orig, GameManager self, GameManager.SceneLoadInfo info)
        {
            orig(self, info);
            _mod.LogModTEMP($"GameManager_BeginSceneTransition: {self.sceneName}");
        }

        private void GameManager_BeginScene(On.GameManager.orig_BeginScene orig, GameManager self)
        {
            orig(self);
            _mod.LogModTEMP($"GameManager_BeginScene: {self.sceneName}");
        }

        private void ModHooks_SceneChanged(string sceneName)
        {
            _mod.LogModTEMP($"ModHooks_SceneChanged: {sceneName}");
            _mod.LogModTEMP($"ModHooks_SceneChanged: scene={GameManager.instance.sceneName}");
        }

        public override void Unload()
        {
            // Don't unload
        }

        private string ModHooks_BeforeSceneLoadHook(string sceneName)
        {
            _mod.LogMod($"ModHooks_BeforeSceneLoadHook: {sceneName}");

            if (sceneName.StartsWith("GG_") && sceneName != "GG_Waterways" && sceneName != "GG_Pipeway")
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
