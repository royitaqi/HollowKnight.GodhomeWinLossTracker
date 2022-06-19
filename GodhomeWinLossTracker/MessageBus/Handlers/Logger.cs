using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class Logger : IHandler
    {
        public void OnMessage(TheMessageBus bus, Modding.Loggable logger, IMessage msg)
        {
            logger.Log($"Message on bus: {msg}");
        }
    }
}
