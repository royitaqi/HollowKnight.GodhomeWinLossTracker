namespace UnitTests
{
    [Serializable]
    public class FolderData_v0_4_3_0
    {
        [JsonProperty("RawRecords")]
        public List<RawWinLoss_v0_4_3_0> RawWinLosses = new();
        public List<RawHit_v0_4_3_0> RawHits = new();
    }

    public enum RecordSources_v0_4_3_0
    {
        Manual = 0,
        Mod = 1,
        Test = 2,
    }

    public enum DamageSources_v0_4_3_0
    {
        Unknown = 0,
        Enemy = 1,
        Hazard = 2,
    }
}
