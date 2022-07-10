﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class BossHP : IMessage
    {
        public int MaxHP { get; set; }
        public int HP { get; set; }

        public override string ToString()
        {
            return $"Boss HP={HP}/{MaxHP}";
        }
    }
}
