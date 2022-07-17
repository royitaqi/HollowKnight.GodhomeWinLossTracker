namespace UnitTests
{
    [TestClass]
    public class TestDisplayInvoker
    {
        private const string SequenceName = "HoG";
        private const string BossName = "The Collector";
        private const string BossSceneName = "GG_Collector_V";
        private readonly RawWinLoss WinHitless100s = new RawWinLoss("", SequenceName, BossName, BossSceneName, 1, 0, 0, 0, 0, 0, 0, 100000, G.RecordSources.Test);
        private readonly RawWinLoss Win1Hit100s = new RawWinLoss("", SequenceName, BossName, BossSceneName, 1, 0, 0, 0, 1, 0, 0, 100000, G.RecordSources.Test);
        private readonly RawWinLoss Win2Hits100s = new RawWinLoss("", SequenceName, BossName, BossSceneName, 1, 0, 0, 0, 2, 0, 0, 100000, G.RecordSources.Test);
        private readonly RawWinLoss WinHitless50s = new RawWinLoss("", SequenceName, BossName, BossSceneName, 1, 0, 0, 0, 0, 0, 0, 50000, G.RecordSources.Test);
        private readonly RawWinLoss Win5Hits50s = new RawWinLoss("", SequenceName, BossName, BossSceneName, 1, 0, 0, 0, 5, 0, 0, 50000, G.RecordSources.Test);

        private IEnumerable<TestUtils.MessageBusTestCase> GetTestCases()
        {
            return new[]
            {
                new TestUtils.MessageBusTestCase
                {
                    Name = "first record, best time, hitless",
                    GlobalData = new G.GlobalData { NotifyForRecord = true },
                    FolderData = new G.FolderData { RawWinLosses = new List<RawWinLoss>() },
                    InputMessages = new IMessage[]
                    {
                        WinHitless100s,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        new BusEvent { ForTest = true, Event = "Won against The Collector in HoG (PB 1:40, hitless)" },
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "first record, best time, 1 hit",
                    GlobalData = new G.GlobalData { NotifyForRecord = true },
                    FolderData = new G.FolderData { RawWinLosses = new List<RawWinLoss>() },
                    InputMessages = new IMessage[]
                    {
                        Win1Hit100s,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        new BusEvent { ForTest = true, Event = "Won against The Collector in HoG (PB 1:40, 1 hit)" },
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "first record, best time, 2 hits",
                    GlobalData = new G.GlobalData { NotifyForRecord = true },
                    FolderData = new G.FolderData { RawWinLosses = new List<RawWinLoss>() },
                    InputMessages = new IMessage[]
                    {
                        Win2Hits100s,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        new BusEvent { ForTest = true, Event = "Won against The Collector in HoG (PB 1:40)" },
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "2nd record, both are better",
                    GlobalData = new G.GlobalData { NotifyForRecord = true },
                    FolderData = new G.FolderData { RawWinLosses = new List<RawWinLoss>() },
                    InputMessages = new IMessage[]
                    {
                        Win2Hits100s,
                        WinHitless50s,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        new BusEvent { ForTest = true, Event = "Won against The Collector in HoG (PB 1:40)" },
                        new BusEvent { ForTest = true, Event = "Won against The Collector in HoG (PB 0:50, hitless)" },
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "2nd record, only time is better",
                    GlobalData = new G.GlobalData { NotifyForRecord = true },
                    FolderData = new G.FolderData { RawWinLosses = new List<RawWinLoss>() },
                    InputMessages = new IMessage[]
                    {
                        Win2Hits100s,
                        Win5Hits50s,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        new BusEvent { ForTest = true, Event = "Won against The Collector in HoG (PB 1:40)" },
                        new BusEvent { ForTest = true, Event = "Won against The Collector in HoG (PB 0:50)" },
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "2nd record, only hits is better",
                    GlobalData = new G.GlobalData { NotifyForRecord = true },
                    FolderData = new G.FolderData { RawWinLosses = new List<RawWinLoss>() },
                    InputMessages = new IMessage[]
                    {
                        Win2Hits100s,
                        Win1Hit100s,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        new BusEvent { ForTest = true, Event = "Won against The Collector in HoG (PB 1:40)" },
                        new BusEvent { ForTest = true, Event = "Won against The Collector in HoG (PB 1 hit)" },
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "2nd record, nothing better",
                    GlobalData = new G.GlobalData { NotifyForRecord = true },
                    FolderData = new G.FolderData { RawWinLosses = new List<RawWinLoss>() },
                    InputMessages = new IMessage[]
                    {
                        Win1Hit100s,
                        Win2Hits100s,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        new BusEvent { ForTest = true, Event = "Won against The Collector in HoG (PB 1:40, 1 hit)" },
                        new BusEvent { ForTest = true, Event = "Won against The Collector in HoG" },
                    },
                },
            };
        }

        [TestMethod]
        public void TestRunAll()
        {
            foreach (var testCase in GetTestCases())
            {
                testCase.HandlersCreator = _ => new Handler[] {
                    new RecordCollector(),
                    new DisplayInvoker(TestUtils.TestLocalizer, _ => { }),
                };
                // Only validate BusEvent messages
                testCase.OutputMessageFilter = msgs => msgs.Where(msg => msg is BusEvent);
                TestUtils.TestMessageBus(testCase);
            }
        }
    }
}