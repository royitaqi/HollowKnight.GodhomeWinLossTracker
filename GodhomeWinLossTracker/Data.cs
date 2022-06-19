using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GodhomeWinLossTracker
{
    [Serializable]
    public class LocalData
    {
        public class WinLoss
        {
            public int Win;
            public int Loss;
        }
        public Dictionary<string, Dictionary<string, WinLoss>> WinLossStats = new();

        public void RegisterWinLoss(Modding.Loggable logger, string sequence, string bossName, bool winLoss)
        {
            if (!WinLossStats.ContainsKey(sequence))
            {
                WinLossStats[sequence] = new();
            }
            if (!WinLossStats[sequence].ContainsKey(bossName))
            {
                WinLossStats[sequence][bossName] = new();
            }
            if (winLoss)
            {
                WinLossStats[sequence][bossName].Win++;
            }
            else
            {
                WinLossStats[sequence][bossName].Loss++;
            }
        }
    }

    [Serializable]
    public class GlobalData
    {
        public int myGlobalData = 999;
    }
}
