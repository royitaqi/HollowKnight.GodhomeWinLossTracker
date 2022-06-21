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
        public WinLossTracker(GodhomeWinLossTracker mod)
        {
            _mod = mod;
        }

        public void OnMessage(TheMessageBus bus, Modding.Loggable logger, IMessage message)
        {
            if (message is FightWinLoss)
            {
                FightWinLoss msg = message as FightWinLoss;
                _mod.localData.RegisterWinLoss(logger, msg.SequenceName, msg.BossName, msg.WinLoss);

                // Put the message back as registered. This should allow display a UI notification in the game.
                bus.Put(new RegisteredFightWinLoss { InnerMessage = msg });
            }
        }

        private readonly GodhomeWinLossTracker _mod;
    }
}
