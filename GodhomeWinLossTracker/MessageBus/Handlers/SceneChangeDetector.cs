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
    internal class SceneChangeDetector: Handler
    {
        public override void Load(TheMessageBus bus, ILogger logger)
        {
            _bus = bus;
            _logger = logger;

            ModHooks.BeforeSceneLoadHook += ModHooks_BeforeSceneLoadHook;
        }

        private string ModHooks_BeforeSceneLoadHook(string sceneName)
        {
#if DEBUG
            _logger.Log($"OnSceneLoad: {sceneName}");
#endif
            _bus.Put(new SceneChange { Name = sceneName });
            return sceneName;
        }

        private TheMessageBus _bus;
        private ILogger _logger;
    }
}
