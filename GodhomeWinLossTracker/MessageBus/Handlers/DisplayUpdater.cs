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

        public new void OnMessage(TheMessageBus bus, Modding.ILogger logger, IMessage message)
        {
            // For RegisteredRawWinLoss, we need to display the inner message.
            if (message is RegisteredRawWinLoss)
            {
                if (_mod.globalData.NotifyForRecord)
                {
                    RawWinLoss record = (message as RegisteredRawWinLoss).InnerMessage;

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
            // For any other types of message, simply display the message itself.
            else if (message is ExportedFolderData)
            {
                if (_mod.globalData.NotifyForExport)
                {
                    ModDisplay.instance.Notify(message.ToString());
                }
            }
        }

        private readonly IGodhomeWinLossTracker _mod;
    }
}
