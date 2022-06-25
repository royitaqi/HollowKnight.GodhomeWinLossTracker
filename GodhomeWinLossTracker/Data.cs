using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using GodhomeWinLossTracker.MessageBus.Messages;

namespace GodhomeWinLossTracker
{
    [Serializable]
    public class FolderData
    {
        public List<RawWinLoss> RawRecords = new();
    }

    [Serializable]
    public class GlobalData
    {
        public bool AutoExport = false;
        public bool NotifyForExport = false;
        public bool NotifyForRecord = true;
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
