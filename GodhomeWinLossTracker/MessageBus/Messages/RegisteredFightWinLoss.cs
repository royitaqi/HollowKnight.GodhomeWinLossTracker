using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class RegisteredFightWinLoss : IMessage
    {
        public FightWinLoss InnerMessage { get; set; }

        public override string ToString()
        {
            Debug.Assert(InnerMessage != null);
            return $"{InnerMessage} (registered)";
        }
    }
}
