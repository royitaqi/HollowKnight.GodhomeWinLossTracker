﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GodhomeWinLossTracker.MessageBus.Messages;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class BossChangeDetector: IHandler
    {
        public void OnMessage(TheMessageBus bus, Modding.Loggable logger, IMessage msg)
        {
            if (msg is SceneChange)
            {
                string sceneName = (msg as SceneChange).Name;
                Debug.Assert(sceneName != null);

                string bossName = GodhomeUtils.GetNullableBossName(sceneName);
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
}
