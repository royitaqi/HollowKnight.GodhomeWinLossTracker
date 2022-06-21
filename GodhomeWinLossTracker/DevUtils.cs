﻿using System;

namespace GodhomeWinLossTracker
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
    }
}