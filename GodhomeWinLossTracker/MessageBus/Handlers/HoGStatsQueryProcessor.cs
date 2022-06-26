﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GodhomeWinLossTracker.MessageBus.Messages;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class HoGStatsQueryProcessor : IHandler
    {
        public HoGStatsQueryProcessor(IGodhomeWinLossTracker mod)
        {
            _mod = mod;
        }

        public void OnMessage(TheMessageBus bus, Modding.ILogger logger, IMessage message)
        {
            if (message is HoGStatsQuery)
            {
                HoGStatsQuery msg = message as HoGStatsQuery;

                // Get all scenes of the boss, ordered.
                // Also get matching prefixes.
                List<string> scenes = new(GodhomeUtils.GetBossScenesByName(msg.BossName));
                if (scenes.Count == 2 && scenes[0].EndsWith("_V"))
                {
                    scenes.Reverse();
                }
                string[] prefixes = scenes.Count == 1 ? new[] { "" } : new[] { "Attuned: ", "Ascended+: " };

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
                        sb.AppendLine($"{prefix}{total} fights, {wins} wins ({100.0 * wins / total,0:F0}%)");
                    }
                }

                // Overwrite text on challenge menu
                msg.Callback(sb.Length != 0 ? sb.ToString() : null);
            }
        }

        private readonly IGodhomeWinLossTracker _mod;
    }
}