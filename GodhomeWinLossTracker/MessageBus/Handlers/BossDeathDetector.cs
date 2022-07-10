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
    internal class BossDeathDetector: Handler
    {
        public override void Load(IGodhomeWinLossTracker mod, TheMessageBus bus)
        {
            _bus = bus;
            _mod = mod;

            On.BossSceneController.EndBossScene += OnEndBossScene;
        }

        private void OnEndBossScene(On.BossSceneController.orig_EndBossScene orig, BossSceneController self)
        {
#if DEBUG
            _mod.Log("OnEndBossScene");
#endif
            // At least one boss died.
            // Note that this event can trigger twice in a fight (e.g. Oro and Mato).
            _bus.Put(new BossDeath());

            orig(self);
        }

        private IGodhomeWinLossTracker _mod;
        private TheMessageBus _bus;
    }
}
