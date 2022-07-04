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
    internal class SequenceChangeDetector: Handler
    {
        public new void OnMessage(TheMessageBus bus, Modding.ILogger logger, IMessage msg)
        {
            // Detect HoG sequence by scene GG_Workshop
            if (msg is SceneChange)
            {
                string currentSceneName = (msg as SceneChange).Name;
                DevUtils.Assert(currentSceneName != null, "currentSceneName shouldn't be null");

                if (currentSceneName == "GG_Workshop")
                {
                    bus.Put(new SequenceChange { Name = "HoG" });
                }
            }
            // Detect pantheon sequences by stats queries
            else if (msg is PantheonStatsQuery)
            {
                int pantheonIndex = (msg as PantheonStatsQuery).PantheonIndex;
                bus.Put(new SequenceChange { Name = $"P{pantheonIndex + 1}" });
            }
        }
    }
}
