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
    internal class WinLossTracker : IHandler
    {
        public WinLossTracker(IGodhomeWinLossTracker mod)
        {
            _mod = mod;
        }

        public void OnMessage(TheMessageBus bus, Modding.ILogger logger, IMessage message)
        {
            if (message is RawWinLoss)
            {
                RawWinLoss msg = message as RawWinLoss;
                _mod.folderData.RawRecords.Add(msg);

                // Put the message back as registered. This should allow display a UI notification in the game.
                bus.Put(new RegisteredRawWinLoss { InnerMessage = msg });
            }
        }

        private readonly IGodhomeWinLossTracker _mod;
    }
}
