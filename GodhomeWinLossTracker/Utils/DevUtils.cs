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
    }
}
