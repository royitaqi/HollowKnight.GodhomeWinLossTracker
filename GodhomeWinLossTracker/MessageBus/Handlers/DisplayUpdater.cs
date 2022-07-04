using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GodhomeWinLossTracker.MessageBus.Messages;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class DisplayUpdater : Handler
    {
        public DisplayUpdater(IGodhomeWinLossTracker mod)
        {
            _mod = mod;
        }

        public void OnRegisteredRawWinLoss(TheMessageBus bus, Modding.ILogger logger, RegisteredRawWinLoss msg)
        {
            // For RegisteredRawWinLoss, we need to display the inner message.
            if (_mod.globalData.NotifyForRecord)
            {
                RawWinLoss record = msg.InnerMessage;

                string pb = "";
                if (_mod.globalData.NotifyPBTime && record.Wins > 0 && record.Losses == 0)
                {
                    var records = _mod.folderData.RawRecords.Where(r => r.SequenceName == record.SequenceName && r.SceneName == record.SceneName && r.Wins > 0 && r.Losses == 0).ToList();
                    if (records.Count >= 10 && records.Min(r => r.FightLengthMs) == record.FightLengthMs)
                    {
                        long minutes = record.FightLengthMs / 1000 / 60;
                        long seconds = record.FightLengthMs / 1000 % 60;
                        pb = $" (PB {minutes}:{seconds:D2})";
                    }
                }

                ModDisplay.instance.Notify(record.ToString() + pb);
            }
        }

        public void OnExportedFolderData(TheMessageBus bus, Modding.ILogger logger, ExportedFolderData msg)
        {
            // For any other types of message, simply display the message itself.
            if (_mod.globalData.NotifyForExport)
            {
                ModDisplay.instance.Notify(msg.ToString());
            }
        }

        private readonly IGodhomeWinLossTracker _mod;
    }
}
