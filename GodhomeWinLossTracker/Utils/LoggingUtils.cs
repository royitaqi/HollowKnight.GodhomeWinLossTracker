using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodhomeWinLossTracker.Utils
{
    internal static class LoggingUtils
    {
        public static Modding.LogLevel LogLevel = Modding.LogLevel.Info;

        // All logs are accepted.
        public static void LogModTEMP(this Modding.ILogger logger, string message)
        {
#if DEBUG
            var time = DateTime.Now.ToString("HH':'mm':'ss'.'fff");
            logger.Log($"{time} [TEMP] {message}");
#endif
        }

        // These logs are accepted:
        // - Unexpected issue but okay to continue.
        public static void LogModWarn(this Modding.ILogger logger, string message)
        {
            if (LogLevel <= Modding.LogLevel.Warn)
            {
                var time = DateTime.Now.ToString("HH':'mm':'ss'.'fff");
                logger.LogWarn($"{time} [W] {message}");
            }
        }

        // These logs are accepted:
        // - The only log for a *manually triggered*, *infrequent* event (a few in a minute; e.g. change scene; start boss fight).
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

        // These logs are accepted:
        // - The more detailed logs for a *manually triggered*, *infrequent* event (a few in a minute; e.g. change scene; start boss fight).
        // - The only log for a *manually triggered*, *frequent* event (once every second; e.g. deal damage; take hits; TK status change).
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

        // These logs are accepted:
        // - The more detailed logs for a *manually triggered*, *frequent* event (once every second; e.g. deal damage; take hits; TK status change).
        // - Automatic events.
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
