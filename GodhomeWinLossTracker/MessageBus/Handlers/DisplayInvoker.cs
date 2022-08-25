using System;
using System.Linq;
using System.Text;
using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class DisplayInvoker : Handler
    {
        public DisplayInvoker(Func<string, string> localizer, Action<string> display)
        {
            _localizer = localizer;
            _display = display;
        }

        public void OnRegisteredRawWinLoss(RegisteredRawWinLoss msg)
        {
            // For RegisteredRawWinLoss, we need to display the inner message.
            if (_mod.globalData.NotifyForRecord)
            {
                RawWinLoss record = msg.InnerMessage;
                DevUtils.Assert((record.Wins == 1) != (record.Losses == 1), "Only 1 win or 1 loss record can be notified");

                // Generate the basic message
                string format = (record.Wins, record.Losses, record.BossPhase) switch {
                    (1, 0, _) => "Notification/Won against {0} in {1}",
                    (0, 1, 0) => "Notification/Lost to {0} in {1}",
                    (0, 1, _) => "Notification/Lost to {0} phase {2} in {1}",
                    _ => throw new AssertionFailedException("Should never arrive here")
                };
                string winLossString = string.Format(
                    _localizer(format),
                    _localizer($"Boss/{record.BossName}"),
                    _localizer($"Sequence/{record.SequenceName}"),
                    record.BossPhase
                );

                // Generate pb messsage
                bool isBetter = false;
                string timeString = null;
                string hitsString = null;
                if (_mod.globalData.NotifyPB && record.Wins > 0 && record.Losses == 0)
                {
                    // Prepare pb strings.
                    // These pb strings can be wiped to null if they are worse than previous records.
                    long minutes = record.FightLengthMs / 1000 / 60;
                    long seconds = record.FightLengthMs / 1000 % 60;
                    timeString = $"{minutes}:{seconds:D2}";
                    hitsString = record.Hits > 1 ? null : _localizer(record.Hits == 0 ? "Notification/hitless" : "Notification/1 hit");

                    // Get all win records before this one
                    var records = _mod.folderData.RawWinLosses
                        .Where(r => r != record && r.SequenceName == record.SequenceName && r.SceneName == record.SceneName && r.Wins > 0 && r.Losses == 0)
                        .ToList();

                    // Compare time
                    if (records.Count == 0)
                    {
                        isBetter = true;
                        // Keep time string
                    }
                    else
                    {
                        var bestTimeSoFar = records.Min(r => r.FightLengthMs);
                        if (record.FightLengthMs < bestTimeSoFar)
                        {
                            isBetter = true;
                            // Keep time string
                        }
                        else if (record.FightLengthMs == bestTimeSoFar)
                        {
                            // Keep time string
                        }
                        else
                        {
                            timeString = null;
                        }
                    }

                    // Compare hits
                    if (records.Count == 0)
                    {
                        isBetter = true;
                    }
                    else
                    {
                        var bestHitsSoFar = records.Min(r => r.Hits);
                        if (record.Hits < bestHitsSoFar)
                        {
                            isBetter = true;
                            // Keep hitsString
                        }
                        else if (record.Hits == bestHitsSoFar)
                        {
                            // Keep hitsString
                        }
                        else
                        {
                            hitsString = null;
                        }
                    }
                }

                // Combine the strings
                StringBuilder sb = new(winLossString);
                // If either time or hits is better, generate PB string
                if (isBetter)
                {
                    if (timeString != null)
                    {
                        if (hitsString != null)
                        {
                            sb.AppendFormat(" ({0} {1}, {2})", _localizer("Notification/PB"), timeString, hitsString);
                        }
                        else
                        {
                            sb.AppendFormat(" ({0} {1})", _localizer("Notification/PB"), timeString);
                        }
                    }
                    else if (hitsString != null)
                    {
                        sb.AppendFormat(" ({0} {1})", _localizer("Notification/PB"), hitsString);
                    }
                    else
                    {
                        // Not generating PB string, because both time and hits strings are null
                    }
                }

                _display(sb.ToString());

#if DEBUG
                _bus.Put(new BusEvent { ForTest = true, Event = sb.ToString() });
#endif
            }
        }

        public void OnExportedFolderData(ExportedFolderData msg)
        {
            // For any other types of message, simply display the message itself.
            if (_mod.globalData.NotifyForExport)
            {
                string text = "Notification/Exported successfully".Localize();
                _display(text);

#if DEBUG
                _bus.Put(new BusEvent { ForTest = true, Event = text });
#endif
            }
        }

        private Func<string, string> _localizer;
        private Action<string> _display;
    }
}
