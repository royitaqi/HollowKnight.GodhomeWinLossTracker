using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modding;

namespace GodhomeWinLossTracker.Utils
{
    internal static class GameManagerUtils
    {
        public static long PlayTimeMs =>
            (long)(
                (
                    (double)ReflectionHelper.GetField<GameManager, float>(GameManager.instance, "sessionStartTime")
                    + (double)ReflectionHelper.GetField<GameManager, float>(GameManager.instance, "sessionPlayTimer")
                ) * 1000
            );
    }
}
