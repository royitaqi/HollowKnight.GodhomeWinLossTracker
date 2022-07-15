using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodhomeWinLossTracker.Utils
{
    internal static class LoggingUtils
    {
        public static Modding.LogLevel LogLevel = Modding.LogLevel.Info;

        // Unexpected issue but okay to continue.
        public static void LogModWarn(this Modding.ILogger logger, string message)
        {
#if DEBUG
            if (LogLevel <= Modding.LogLevel.Warn)
            {
                var time = DateTime.Now.ToString("HH':'mm':'ss'.'fff");
                logger.Log($"{time} [W] {message}");
            }
#endif
        }

        // Manually triggered, *infrequent* events (a few in a minute).
        // One log for each user input.
        public static void LogMod(this Modding.ILogger logger, string message)
        {
#if DEBUG
            if (LogLevel <= Modding.LogLevel.Info)
            {
                var time = DateTime.Now.ToString("HH':'mm':'ss'.'fff");
                logger.Log($"{time} [I] {message}");
            }
#endif
        }

        // Manually triggered, *frequent* events (once every second).
        // One log for each user input.
        public static void LogModDebug(this Modding.ILogger logger, string message)
        {
#if DEBUG
            if (LogLevel <= Modding.LogLevel.Debug)
            {
                var time = DateTime.Now.ToString("HH':'mm':'ss'.'fff");
                logger.Log($"{time} [D] {message}");
            }
#endif
        }

        // Multiple logs for each user input OR automatic events.
        public static void LogModFine(this Modding.ILogger logger, string message)
        {
#if DEBUG
            if (LogLevel <= Modding.LogLevel.Fine)
            {
                var time = DateTime.Now.ToString("HH':'mm':'ss'.'fff");
                logger.Log($"{time} [F] {message}");
            }
#endif
        }

        public static void CountedLog(string log)
        {
            if (_countedLog.ContainsKey(log))
            {
                _countedLog[log]++;
            }
            else
            {
                _countedLog[log] = 1;
            }
        }

        public static string DumpLogCount()
        {
            StringBuilder sb = new();
            foreach (var kvp in _countedLog.OrderByDescending(kvp => kvp.Value))
            {
                sb.AppendLine($"CountedLog: {kvp.Key}: {kvp.Value}");
            }
            _countedLog.Clear();
            return sb.ToString();
        }

        private static Dictionary<string, int> _countedLog = new();
    }

    internal class ModLogLevelAttribute : Attribute
    {
        public ModLogLevelAttribute(Modding.LogLevel logLevel)
        {
            LogLevel = logLevel;
        }

        public Modding.LogLevel LogLevel { get; private set; }
    }
}
