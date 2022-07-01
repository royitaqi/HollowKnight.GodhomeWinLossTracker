using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class GameLoaded : IMessage
    {
        public override string ToString()
        {
            return "Game loaded";
        }
    }
}
