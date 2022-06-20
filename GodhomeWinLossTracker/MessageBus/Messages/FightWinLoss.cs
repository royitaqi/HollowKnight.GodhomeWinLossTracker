﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class FightWinLoss : IMessage
    {
        public string SequenceName { get; set; }
        // Fight win/loss are tracked by boss names, not boss scene names.
        public string BossName { get; set; }
        public bool WinLoss { get; set; }

        public override string ToString()
        {
            DevUtils.Assert(SequenceName != null, "SequenceName shouldn't be null");
            DevUtils.Assert(BossName != null, "BossName shouldn't be null");
            string verb = WinLoss ? "Won" : "Lost to";
            return $"{verb} {BossName} in {SequenceName}";
        }
    }
}
