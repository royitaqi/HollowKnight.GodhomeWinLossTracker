using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodhomeWinLossTracker.Utils
{
    internal class AssertionFailedException : ApplicationException
    {
        public AssertionFailedException(string msg) : base(msg) { }
    }

    internal static class DevUtils
    {
        public static void Assert(bool condition, string message)
        {
#if DEBUG
            if (!condition)
            {
                throw new AssertionFailedException(message);
            }
#endif
        }

        public static long GetTimestampEpochMs()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public static void Log(string log)
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

        public static string DumpLog()
        {
            StringBuilder sb = new();
            foreach (var kvp in _countedLog.OrderByDescending(kvp => kvp.Value))
            {
                sb.AppendLine($"{kvp.Key}: {kvp.Value}");
            }
            _countedLog.Clear();
            return sb.ToString();
        }

        private static Dictionary<string, int> _countedLog = new();
    }
}
