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
                int index = msg.PantheonIndex;

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
                if (records.Count == 0)
                {
                    // There is no records at all. Don't call callback.
                    return;
                }

                List<string> scenes = new(GodhomeUtils.GetPantheonScenes(index));
                DevUtils.Assert(scenes.Count > 0, "Pantheons should have at least one boss scene");

                // Generate "Runs" based on the first boss' fights.
                int runs = records.Max(r => r.Total);
                if (runs == 0)
                {
                    // There is no run at all. Don't call callback.
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
                    pb = $"PB: {GodhomeUtils.GetNullableBossNameBySceneName(furthestWonScene)}";
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
                        sb.Append($"{GodhomeUtils.GetNullableBossNameBySceneName(churns[i].SceneName)} {churns[i].WinRate * 100,0:F0}%");
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
                runsText ??= "null";
                pb ??= "null";
                churnsText ??= "null";
                bus.Put(new BusEvent { ForTest = true, Event = $"{runsText} | {pb} | {churnsText}" });
#endif
            }
        }

        private readonly IGodhomeWinLossTracker _mod;
    }
}
