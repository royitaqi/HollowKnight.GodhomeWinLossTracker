namespace UnitTests
{
    [Serializable]
    public class FolderData_v0_2_1_0
    {
        public List<RawWinLoss_v0_2_1_0> RawRecords = new();
    }

    public enum Sources_v0_2_1_0 : int
    {
        Manual = 0,
        Mod = 1,
    }
}
