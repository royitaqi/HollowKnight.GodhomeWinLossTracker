using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class BossChangeDetector: Handler
    {
        public void OnSceneChange(SceneChange msg)
        {
            string sceneName = msg.Name;
            DevUtils.Assert(sceneName != null, "sceneName shouldn't be null");

            string bossName = GodhomeUtils.GetNullableBossNameBySceneName(sceneName);
            if (bossName != null)
            {
                _bus.Put(new BossChange(bossName, sceneName));
            }
            else
            {
                _bus.Put(new BossChange());
            }
        }
    }
}
