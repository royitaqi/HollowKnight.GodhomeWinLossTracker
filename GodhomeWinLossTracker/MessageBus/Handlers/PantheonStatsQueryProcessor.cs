using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GodhomeWinLossTracker.MessageBus.Messages;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class PantheonStatsQueryProcessor : IHandler
    {
        public PantheonStatsQueryProcessor(IGodhomeWinLossTracker mod)
        {
            _mod = mod;
        }

        public void OnMessage(TheMessageBus bus, Modding.ILogger logger, IMessage message)
        {
            if (message is PantheonStatsQuery)
            {
                PantheonStatsQuery msg = message as PantheonStatsQuery;

                int? indexq = GodhomeUtils.GetPantheonIndex(msg.PantheonName);
                if (indexq == null)
                {
                    // Unknown pantheon. Return without calling callback.
                    return;
                }
                int index = (int)indexq;

                string sequenceName = $"P{index + 1}";
                var records = _mod.folderData.RawRecords
                    .Where(r => r.SequenceName == sequenceName)
                    .GroupBy(
                        r => r.SceneName,
                        r => r,
                        (scene, records) => {
                            int wins = records.Sum(r => r.Wins);
                            int total = records.Sum(r => r.Wins + r.Losses);
                            float? winRate = total > 0 ? (float)wins / total : null;
                            return new { SceneName = scene, Wins = wins, Total = total, WinRate = winRate };
                        }
                    )
                    .ToList();

                List<string> scenes = new(GodhomeUtils.GetPantheonScenes(index));
                DevUtils.Assert(scenes.Count > 0, "Pantheons should have at least one boss scene");

                // Generate "Runs" based on the first boss' fights.
                int runs = records.Where(r => r.SceneName == scenes[0]).Sum(r => r.Total);
                if (runs == 0)
                {
                    // No runs yet, don't ruin the default text.
                    return;
                }
                string runsText = $"Runs: {runs}";

                // Generate "PB"
                string pb = null;
                string furthestWonScene = scenes.LastOrDefault(s => records.Any(r => r.SceneName == s && r.Wins > 0));
                // Never win a boss in previous runs. No PB.
                if (furthestWonScene == null)
                {
                    // Nothing
                }
                // Won last boss before.
                else if (furthestWonScene == scenes.Last())
                {
                    pb = "PB: Win";
                }
                // Generate PB for the furthest boss won
                else
                {
                    pb = $"PB: {GodhomeUtils.GetNullableBossName(furthestWonScene)}";
                }

                // Generate "Biggest churns"
                var churns = records.Where(r => r.Total > 0 && r.Wins != r.Total).OrderBy(r => r.WinRate).ThenByDescending(r => r.Total).Take(3).ToList();
                string churnsText = null;
                if (churns.Count != 0)
                {
                    StringBuilder sb = new($"Top churns: ");
                    for (int i = 0; i < churns.Count; i++)
                    {
                        if (i != 0)
                        {
                            sb.Append(", ");
                        }
                        sb.Append($"{GodhomeUtils.GetNullableBossName(churns[i].SceneName)} {churns[i].WinRate * 100,0:F0}%");
                    }
                    churnsText = sb.ToString();
                }

                // Overwrite text on challenge menu
                msg.Callback(
                    runsText,
                    pb,
                    churnsText
                );
#if DEBUG
                bus.Put(new BusEvent { ForTest = true, Event = $"{runsText} | {pb} | {churnsText}" });
#endif
            }
        }

        private readonly IGodhomeWinLossTracker _mod;
    }
}
