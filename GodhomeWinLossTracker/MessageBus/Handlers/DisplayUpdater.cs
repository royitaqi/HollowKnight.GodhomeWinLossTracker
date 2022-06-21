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
            if (message is RegisteredRawWinLoss)
            {
                ModDisplay.instance.Notify((message as RegisteredRawWinLoss).InnerMessage.ToString());
            }
        }
    }
}
