using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodhomeWinLossTracker.MessageBus
{
    internal interface IHandler
    {
        abstract void OnMessage(TheMessageBus bus, Modding.ILogger logger, IMessage msg);
    }
}
