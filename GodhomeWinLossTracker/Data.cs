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

    [Serializable]
    public class SaveData
    {
        public List<RawWinLoss> RawWinLossRecords = new();
    }

    public class RawWinLoss
    {
        public long EpochMs { get; set; }
        public string SequenceName { get; set; }
        public string BossName { get; set; }
        public string SceneName { get; set; }
        public bool WinLoss { get; set; }
        public long FightLengthMs { get; set; }
    }
}
