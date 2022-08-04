using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class BossDeathObserver: Handler
    {
        public override void Load(IGodhomeWinLossTracker mod, TheMessageBus bus, Modding.ILogger logger)
        {
            base.Load(mod, bus, logger);
            On.BossSceneController.EndBossScene += OnEndBossScene;
        }

        public override void Unload()
        {
            base.Unload();
            On.BossSceneController.EndBossScene -= OnEndBossScene;
        }

        private void OnEndBossScene(On.BossSceneController.orig_EndBossScene orig, BossSceneController self)
        {
            _mod.LogModDebug("OnEndBossScene");

            // At least one boss died.
            // Note that this event can trigger twice in a fight (e.g. Oro and Mato).
            _bus.Put(new BossDeath());

            orig(self);
        }
    }
}
