using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using GodhomeWinLossTracker.MessageBus.Messages;

namespace GodhomeWinLossTracker
{
    public enum RecordSources
    {
        Manual = 0,
        Mod = 1,
        Test = 2,
    }

    [Serializable]
    public class FolderData
    {
        public List<RawWinLoss> RawRecords = new();
    }

    [Serializable]
    public class GlobalData
    {
        public bool ShowStatsInChallengeMenu = true;
        public bool NotifyForRecord = true;
        public bool NotifyPBTime = true;
        public bool NotifyForExport = false;
        public bool AutoExport = false;
    }

    [Serializable]
    public class LocalData
    {
        public int ProfileID = 0;
    }

    internal interface IDataHolder
    {
        public GlobalData globalData { get; set; }
        public LocalData localData { get; set; }
        public FolderData folderData { get; set; }
    }
}
