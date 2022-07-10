using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;
using Modding;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class SceneChangeObserver: Handler
    {
        public override void Load(IGodhomeWinLossTracker mod, TheMessageBus bus)
        {
            base.Load(mod, bus);
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
