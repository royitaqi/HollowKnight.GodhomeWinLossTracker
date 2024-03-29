﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class HoGStatsQueryProcessor : Handler
    {
        public HoGStatsQueryProcessor(Func<string, string> localizer)
        {
            _localizer = localizer;
        }

        public void OnHoGStatsQuery(HoGStatsQuery msg)
        {
            // Get all scenes of the boss, ordered.
            // Also get matching prefixes.
            List<string> scenes = new(GodhomeUtils.GetBossScenesByName(msg.BossName));
            if (scenes.Count == 2 && scenes[0].EndsWith("_V"))
            {
                scenes.Reverse();
            }
            string[] prefixes = scenes.Count == 1 ? new[] { "" } : new[] { _localizer("StatsDisplay/HoG/Attuned") + ": ", _localizer("StatsDisplay/HoG/Ascended+") + ": " };

            // Print stats into a string.
            StringBuilder sb = new();
            for (int i = 0; i < scenes.Count; i++)
            {
                string scene = scenes[i];
                string prefix = prefixes[i];

                var records = _mod.folderData.RawWinLosses.Where(r => r.SequenceName == "HoG" && r.SceneName == scene);
                int wins = records.Sum(r => r.Wins);
                int total = records.Sum(r => r.Wins + r.Losses);

                if (total != 0)
                {
                    sb.AppendLine(String.Format(
                        _localizer("StatsDisplay/HoG/{0}{1} fights, {2} wins ({3,0:F0}%)"),
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
            _bus.Put(new BusEvent { ForTest = true, Event = text });
#endif
        }

        private readonly Func<string, string> _localizer;
    }
}
