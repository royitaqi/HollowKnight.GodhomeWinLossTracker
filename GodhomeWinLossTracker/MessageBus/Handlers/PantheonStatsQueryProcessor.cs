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
    internal class PantheonStatsQueryProcessor : Handler
    {
        public PantheonStatsQueryProcessor(IGodhomeWinLossTracker mod, Func<string, string> localizer)
        {
            _mod = mod;
            _localizer = localizer;
        }

        public void OnPantheonStatsQuery(TheMessageBus bus, Modding.ILogger logger, PantheonStatsQuery msg)
        {
            // If the pantheon is unidentified, don't call callback.
            int? indexq = msg.PantheonIndex;
            if (indexq == null)
            {
                return;
            }

            // If it's the p5 segment portal in GodSeeker+, don't call callback.
            if (msg.PantheonAttribute == GodhomeUtils.PantheonAttributes.IsSegment)
            {
                return;
            }

            int index = (int)indexq;
            string sequenceName = GodhomeUtils.GetPantheonSequenceName(index, msg.PantheonAttribute);
            var records = _mod.folderData.RawWinLosses
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
            string runsText = String.Format(_localizer("StatsDisplay/Pantheons/Runs: {0}"), runs);

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
                pb = _localizer("StatsDisplay/Pantheons/PB: Win");
            }
            // Generate PB for the furthest boss won
            else
            {
                pb = String.Format(
                    _localizer("StatsDisplay/Pantheons/PB: {0}"),
                    _localizer($"Boss/{GodhomeUtils.GetNullableBossNameBySceneName(furthestWonScene)}")
                );
            }

            // Generate "Biggest churns"
            var churns = records.Where(r => r.Total > 0 && r.Wins != r.Total).OrderBy(r => r.WinRate).ThenByDescending(r => r.Total).Take(3).ToList();
            string churnsText = null;
            if (churns.Count != 0)
            {
                StringBuilder sb = new(_localizer("StatsDisplay/Pantheons/Top churns") + ": ");
                for (int i = 0; i < churns.Count; i++)
                {
                    if (i != 0)
                    {
                        sb.Append(", ");
                    }
                    sb.AppendFormat(
                        "{0} {1,0:F0}%",
                        _localizer($"Boss/{GodhomeUtils.GetNullableBossNameBySceneName(churns[i].SceneName)}"),
                        churns[i].WinRate * 100
                    );
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

        private readonly IGodhomeWinLossTracker _mod;
        private readonly Func<string, string> _localizer;
    }
}
