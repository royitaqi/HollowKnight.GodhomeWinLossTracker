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
    internal class HoGStatsQueryProcessor : Handler
    {
        public HoGStatsQueryProcessor(IGodhomeWinLossTracker mod)
        {
            _mod = mod;
        }

        public void OnHoGStatsQuery(TheMessageBus bus, Modding.ILogger logger, HoGStatsQuery msg)
        {
            // Get all scenes of the boss, ordered.
            // Also get matching prefixes.
            List<string> scenes = new(GodhomeUtils.GetBossScenesByName(msg.BossName));
            if (scenes.Count == 2 && scenes[0].EndsWith("_V"))
            {
                scenes.Reverse();
            }
            string[] prefixes = scenes.Count == 1 ? new[] { "" } : new[] { "StatsDisplay/HoG/Attuned".Localize() + ": ", "StatsDisplay/HoG/Ascended+".Localize() + ": " };

            // Print stats into a string.
            StringBuilder sb = new();
            for (int i = 0; i < scenes.Count; i++)
            {
                string scene = scenes[i];
                string prefix = prefixes[i];

                var records = _mod.folderData.RawRecords.Where(r => r.SequenceName == "HoG" && r.SceneName == scene);
                int wins = records.Sum(r => r.Wins);
                int total = records.Sum(r => r.Wins + r.Losses);

                if (total != 0)
                {
                    sb.AppendLine(String.Format(
                        "StatsDisplay/HoG/{0}{1} fights, {2} wins ({3,0:F0}%)".Localize(),
                        prefix,
                        total,
                        wins,
                        100.0 * wins / total
                    ));
                }
            }

            // Overwrite text on challenge menu
            string text = sb.Length != 0 ? sb.ToString() : null;
            msg.Callback(text);

#if DEBUG
            // For unittesting
            text ??= "null";
            bus.Put(new BusEvent { ForTest = true, Event = text });
#endif
        }

        private readonly IGodhomeWinLossTracker _mod;
    }
}
