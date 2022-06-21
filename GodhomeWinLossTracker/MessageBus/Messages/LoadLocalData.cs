using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class LoadLocalData : IMessage
    {
        public override string ToString()
        {
            return "Load local data";
        }
    }
}
