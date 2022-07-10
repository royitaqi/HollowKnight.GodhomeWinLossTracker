using System;
using System.Reflection;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class Logger : Handler
    {
        public override void OnMessage(TheMessageBus bus, Modding.ILogger logger, IMessage msg)
        {
            string log = $"Message on bus: {msg.GetType().Name}: {msg}";

            var logLevel = msg.GetType().GetCustomAttribute<ModLogLevelAttribute>()?.LogLevel;
            Action<string> logMethod = logLevel switch
            {
                Modding.LogLevel.Info or null => logger.LogMod,
                Modding.LogLevel.Debug => logger.LogModDebug,
                Modding.LogLevel.Fine => logger.LogModFine,
                _ => throw new AssertionFailedException("Should never arrive here"),
            };

            logMethod(log);
        }
    }
}
