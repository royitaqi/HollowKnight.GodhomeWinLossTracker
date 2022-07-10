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
    internal class DisplayInvoker : Handler
    {
        public DisplayInvoker(IGodhomeWinLossTracker mod)
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
                    (record.Wins > 0 ? "Notification/Won {0} in {1}" : "Notification/Loss {0} in {1}").Localize(),
                    $"Boss/{record.BossName}".Localize(),
                    $"Sequence/{record.SequenceName}".Localize()
                );

                string pbString = "";
                if (_mod.globalData.NotifyPBTime && record.Wins > 0 && record.Losses == 0)
                {
                    var records = _mod.folderData.RawWinLosses.Where(r => r.SequenceName == record.SequenceName && r.SceneName == record.SceneName && r.Wins > 0 && r.Losses == 0).ToList();
                    if (records.Count >= 10 && records.Min(r => r.FightLengthMs) == record.FightLengthMs)
                    {
                        long minutes = record.FightLengthMs / 1000 / 60;
                        long seconds = record.FightLengthMs / 1000 % 60;
                        pbString = " (" + "Notification/PB".Localize() + $" {minutes}:{seconds:D2})";
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
                ModDisplay.instance.Notify("Notification/Exported successfully".Localize());
            }
        }

        private readonly IGodhomeWinLossTracker _mod;
    }
}
