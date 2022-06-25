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
        public void OnMessage(TheMessageBus bus, Modding.ILogger logger, IMessage msg)
        {
            logger.Log($"Message on bus: {msg}");
            GameManager gm = GameManager.instance;
            logger.Log($"DEBUG gm.PlayTime = {gm.PlayTime,0:F5}");
            logger.Log($"DEBUG DevUtils.GetTimestampEpochMs() = {DevUtils.GetTimestampEpochMs()}");
        }
    }
}
