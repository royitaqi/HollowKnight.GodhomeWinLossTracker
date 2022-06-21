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
        public void OnMessage(TheMessageBus bus, Modding.Loggable logger, IMessage message)
        {
            // For RegisteredRawWinLoss, we need to display the inner message.
            if (message is RegisteredRawWinLoss)
            {
                ModDisplay.instance.Notify((message as RegisteredRawWinLoss).InnerMessage.ToString());
            }
            // For any other types of message, simply display the message itself.
            else if (message is ExportedFolderData)
            {
                ModDisplay.instance.Notify(message.ToString());
            }
        }
    }
}
