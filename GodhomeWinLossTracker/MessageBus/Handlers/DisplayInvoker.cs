using System.Linq;
using System.Text;
using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class DisplayInvoker : Handler
    {
        public void OnRegisteredRawWinLoss(RegisteredRawWinLoss msg)
        {
            // For RegisteredRawWinLoss, we need to display the inner message.
            if (_mod.globalData.NotifyForRecord)
            {
                RawWinLoss record = msg.InnerMessage;
                DevUtils.Assert(record.Wins > 0 != record.Losses > 0, "Only 1 win or 1 loss record can be notified");

                // Generate the basic message
                string winLossString = string.Format(
                    (record.Wins > 0 ? "Notification/Won {0} in {1}" : "Notification/Loss {0} in {1}").Localize(),
                    $"Boss/{record.BossName}".Localize(),
                    $"Sequence/{record.SequenceName}".Localize()
                );

                // Generate pb time
                string pbString = null;
                if (_mod.globalData.NotifyPBTime && record.Wins > 0 && record.Losses == 0)
                {
                    var records = _mod.folderData.RawWinLosses.Where(r => r.SequenceName == record.SequenceName && r.SceneName == record.SceneName && r.Wins > 0 && r.Losses == 0).ToList();
                    if (records.Count >= 10 && records.Min(r => r.FightLengthMs) == record.FightLengthMs)
                    {
                        long minutes = record.FightLengthMs / 1000 / 60;
                        long seconds = record.FightLengthMs / 1000 % 60;
                        pbString = "Notification/PB".Localize() + $" {minutes}:{seconds:D2}";
                    }
                }

                // Generate hits
                string hitString = null;
                if (record.Wins > 0 && record.Losses == 0 && record.Hits < 2)
                {
                    hitString = record.Hits == 0 ? "Notification/Hitless".Localize() : "Notification/Hit 1".Localize();
                }

                // Combine the strings
                StringBuilder sb = new(winLossString);
                if (pbString != null)
                {
                    if (hitString != null)
                    {
                        sb.AppendFormat(" ({0}, {1})", pbString, hitString);
                    }
                    else
                    {
                        sb.AppendFormat(" ({0})", pbString);
                    }
                }
                else if (hitString != null)
                {
                    sb.AppendFormat(" ({0})", hitString);
                }

                ModDisplay.instance.Notify(sb.ToString());
            }
        }

        public void OnExportedFolderData(ExportedFolderData msg)
        {
            // For any other types of message, simply display the message itself.
            if (_mod.globalData.NotifyForExport)
            {
                ModDisplay.instance.Notify("Notification/Exported successfully".Localize());
            }
        }
    }
}
