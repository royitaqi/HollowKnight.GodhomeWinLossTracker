﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class SceneChange: IMessage
    {
        public string Name { get; set; }

        public override string ToString()
        {
            string displayName = Name ?? "null";
            return $"Scene changed to {displayName}";
        }
    }
}
