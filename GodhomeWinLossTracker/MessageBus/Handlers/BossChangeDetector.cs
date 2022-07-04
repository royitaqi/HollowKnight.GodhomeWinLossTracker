using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class BossChangeDetector: Handler
    {
        public void OnSceneChange(TheMessageBus bus, Modding.ILogger logger, SceneChange msg)
        {
            string sceneName = msg.Name;
            DevUtils.Assert(sceneName != null, "sceneName shouldn't be null");

            string bossName = GodhomeUtils.GetNullableBossNameBySceneName(sceneName);
            if (bossName != null)
            {
                bus.Put(new BossChange(bossName, sceneName));
            }
            else
            {
                bus.Put(new BossChange());
            }
        }
    }
}
