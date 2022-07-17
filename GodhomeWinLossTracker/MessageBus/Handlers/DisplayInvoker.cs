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
                DevUtils.Assert(record.Wins > 0 != record.Losses > 0, "Only 1 win or 1 loss record can be notified");

                // Generate the basic message
                string winLossString = string.Format(
                    _localizer(record.Wins > 0 ? "Notification/Won against {0} in {1}" : "Notification/Loss to {0} in {1}"),
                    _localizer($"Boss/{record.BossName}"),
                    _localizer($"Sequence/{record.SequenceName}")
                );

                // Generate pb messsage
                string timeString = null;
                string hitString = null;
                if (_mod.globalData.NotifyPB && record.Wins > 0 && record.Losses == 0)
                {
                    // Get all win records before this one
                    var records = _mod.folderData.RawWinLosses.Where(r => r != record && r.SequenceName == record.SequenceName && r.SceneName == record.SceneName && r.Wins > 0 && r.Losses == 0).ToList();

                    // Generate time
                    if (records.Count == 0 || record.FightLengthMs < records.Min(r => r.FightLengthMs))
                    {
                        long minutes = record.FightLengthMs / 1000 / 60;
                        long seconds = record.FightLengthMs / 1000 % 60;
                        timeString = $"{minutes}:{seconds:D2}";
                    }

                    // Generate hits
                    if (record.Hits < 2 && (records.Count == 0 || record.Hits < records.Min(r => r.Hits)))
                    {
                        hitString = _localizer(record.Hits == 0 ? "Notification/hitless" : "Notification/1 hit");
                    }
                }

                // Combine the strings
                StringBuilder sb = new(winLossString);
                if (timeString != null)
                {
                    if (hitString != null)
                    {
                        sb.AppendFormat(" ({0} {1}, {2})", _localizer("Notification/PB"), timeString, hitString);
                    }
                    else
                    {
                        sb.AppendFormat(" ({0} {1})", _localizer("Notification/PB"), timeString);
                    }
                }
                else if (hitString != null)
                {
                    sb.AppendFormat(" ({0} {1})", _localizer("Notification/PB"), hitString);
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
