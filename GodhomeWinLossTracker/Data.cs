using System;
using System.Collections.Generic;
using GodhomeWinLossTracker.MessageBus.Messages;
using Newtonsoft.Json;

namespace GodhomeWinLossTracker
{
    [Serializable]
    public class FolderData
    {
        [JsonProperty("RawRecords")]
        public List<RawWinLoss> RawWinLosses = new();
        public List<RawHit> RawHits = new();
        public List<RawPhase> RawPhases = new();
    }

    [Serializable]
    public class GlobalData
    {
        public bool ShowStatsInChallengeMenu = true;
        public bool NotifyForRecord = true;
        public bool NotifyPB = true;
        public bool NotifyForExport = false;
        public bool AutoExport = false;
        public bool AsyncWrites = false;
        public bool AutoScreenCapture = false;
        public List<AutoScreenCaptureConfig> AutoScreenCaptureConfigs = new();
    }

    [Serializable]
    public class LocalData
    {
    }

    internal interface IDataHolder
    {
        public GlobalData globalData { get; set; }
        public LocalData localData { get; set; }
        public FolderData folderData { get; set; }
    }

    public enum RecordSources
    {
        Manual = 0,
        Mod = 1,
        Test = 2,
    }

    internal class VersionedFolderData
    {
        public string Version { get; set; }
        public FolderData FolderData { get; set; }
    }

    public class AutoScreenCaptureConfig
    {
        public enum Triggers
        {
            Hits = 0,
        }
        public Triggers Trigger { get; set; }
        public string SequenceName { get; set; }
        public string SceneName { get; set; }
        public string DamageSource { get; set; }
        public int BossPhase { get; set; }
    }
}
