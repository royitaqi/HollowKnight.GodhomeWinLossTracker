using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class SaveLocalData : IMessage
    {
        public override string ToString()
        {
            return "Save local data";
        }
    }
}
