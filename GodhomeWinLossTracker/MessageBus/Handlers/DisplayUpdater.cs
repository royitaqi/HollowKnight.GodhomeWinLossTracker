﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;

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

                string recordString = string.Format(
                    (record.Wins > 0 ? "Display/Won {0} in {1}" : "Display/Loss {0} in {1}").Localize(),
                    $"Boss/{record.BossName}".Localize(),
                    $"Sequence/{record.SequenceName}".Localize()
                );

                string pbString = "";
                if (_mod.globalData.NotifyPBTime && record.Wins > 0 && record.Losses == 0)
                {
                    var records = _mod.folderData.RawRecords.Where(r => r.SequenceName == record.SequenceName && r.SceneName == record.SceneName && r.Wins > 0 && r.Losses == 0).ToList();
                    if (records.Count >= 10 && records.Min(r => r.FightLengthMs) == record.FightLengthMs)
                    {
                        long minutes = record.FightLengthMs / 1000 / 60;
                        long seconds = record.FightLengthMs / 1000 % 60;
                        pbString = " (" + "Display/PB".Localize() + $" {minutes}:{seconds:D2})";
                    }
                }

                ModDisplay.instance.Notify(recordString + pbString);
            }
        }

        public void OnExportedFolderData(TheMessageBus bus, Modding.ILogger logger, ExportedFolderData msg)
        {
            // For any other types of message, simply display the message itself.
            if (_mod.globalData.NotifyForExport)
            {
                string text = string.Format(
                    "Display/Exported to {0}".Localize(),
                    msg.Filename
                );
                ModDisplay.instance.Notify(text);
            }
        }

        private readonly IGodhomeWinLossTracker _mod;
    }
}
