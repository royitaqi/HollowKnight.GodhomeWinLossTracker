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
    internal class DisplayUpdater : IHandler
    {
        public DisplayUpdater(IGodhomeWinLossTracker mod)
        {
            _mod = mod;
        }

        public void OnMessage(TheMessageBus bus, Modding.ILogger logger, IMessage message)
        {
            logger.Log($"DEBUG _mod.globalData.NotifyForRecord = {_mod.globalData.NotifyForRecord}");
            logger.Log($"DEBUG _mod.globalData.NotifyForExport = {_mod.globalData.NotifyForExport}");

            // For RegisteredRawWinLoss, we need to display the inner message.
            if (message is RegisteredRawWinLoss)
            {
                if (_mod.globalData.NotifyForRecord)
                {
                    ModDisplay.instance.Notify((message as RegisteredRawWinLoss).InnerMessage.ToString());
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
