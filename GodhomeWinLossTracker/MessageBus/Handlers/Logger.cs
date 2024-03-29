﻿using System;
using System.Reflection;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class Logger : Handler
    {
        public override void OnMessage(IMessage msg)
        {
            LogMessage(msg);

            // Still call base handler to process any message
            base.OnMessage(msg);
        }

        public override void Unload()
        {
            // Don't unload
        }

        private void LogMessage(IMessage msg)
        {
            string log = $"Message on bus: {msg.GetType().Name}: {msg}";

            var logLevel = msg.GetType().GetCustomAttribute<ModLogLevelAttribute>()?.LogLevel;
            Action<string> logMethod = logLevel switch
            {
                Modding.LogLevel.Info or null => _logger.LogMod,
                Modding.LogLevel.Debug => _logger.LogModDebug,
                Modding.LogLevel.Fine => _logger.LogModFine,
                _ => throw new AssertionFailedException("Should never arrive here"),
            };

            logMethod(log);
        }
    }
}
