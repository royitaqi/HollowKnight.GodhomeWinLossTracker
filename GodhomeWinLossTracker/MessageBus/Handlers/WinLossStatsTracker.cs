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
    internal class WinLossStatsTracker : IHandler
    {
        public WinLossStatsTracker(GodhomeWinLossTracker mod)
        {
            _mod = mod;
        }

        public void OnMessage(TheMessageBus bus, Modding.Loggable logger, IMessage message)
        {
            if (message is FightWinLoss)
            {
                FightWinLoss msg = message as FightWinLoss;
                if (!msg.Registered)
                {
                    _mod.localData.RegisterWinLoss(logger, "HoG", msg.BossName, msg.WinLoss);

                    // Put the message back as registered (for logging purpose).
                    msg.Registered = true;
                    bus.Put(msg);
                }
            }
        }

        private readonly GodhomeWinLossTracker _mod;
    }
}
