namespace UnitTests
{
    [Serializable]
    public class FolderData_v0_3_0_2
    {
        public List<RawWinLoss_v0_3_0_2> RawRecords = new();
    }

    public enum Sources_v0_3_0_2 : int
    {
        Manual = 0,
        Mod = 1,
    }
}
