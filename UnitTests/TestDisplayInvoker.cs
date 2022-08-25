namespace UnitTests
{
    [TestClass]
    public class TestDisplayInvoker
    {
        private const string SequenceName = "HoG";
        private const string BossName = "The Collector";
        private const string BossSceneName = "GG_Collector_V";
        private const string BossName2 = "Absolute Radiance";
        private const string BossSceneName2 = "GG_Radiance";
        private readonly RawWinLoss WinHitless150s = new RawWinLoss("", SequenceName, BossName, BossSceneName, 1, 0, 0, 0, 0, 0, 0, 150000, G.RecordSources.Test, 0);
        private readonly RawWinLoss Win1Hit150s = new RawWinLoss("", SequenceName, BossName, BossSceneName, 1, 0, 0, 0, 1, 0, 0, 150000, G.RecordSources.Test, 0);
        private readonly RawWinLoss Win2Hits150s = new RawWinLoss("", SequenceName, BossName, BossSceneName, 1, 0, 0, 0, 2, 0, 0, 150000, G.RecordSources.Test, 0);
        private readonly RawWinLoss WinHitless100s = new RawWinLoss("", SequenceName, BossName, BossSceneName, 1, 0, 0, 0, 0, 0, 0, 100000, G.RecordSources.Test, 0);
        private readonly RawWinLoss Win1Hit100s = new RawWinLoss("", SequenceName, BossName, BossSceneName, 1, 0, 0, 0, 1, 0, 0, 100000, G.RecordSources.Test, 0);
        private readonly RawWinLoss Win2Hits100s = new RawWinLoss("", SequenceName, BossName, BossSceneName, 1, 0, 0, 0, 2, 0, 0, 100000, G.RecordSources.Test, 0);
        private readonly RawWinLoss WinHitless50s = new RawWinLoss("", SequenceName, BossName, BossSceneName, 1, 0, 0, 0, 0, 0, 0, 50000, G.RecordSources.Test, 0);
        private readonly RawWinLoss Win1Hit50s = new RawWinLoss("", SequenceName, BossName, BossSceneName, 1, 0, 0, 0, 1, 0, 0, 50000, G.RecordSources.Test, 0);
        private readonly RawWinLoss Win2Hits50s = new RawWinLoss("", SequenceName, BossName, BossSceneName, 1, 0, 0, 0, 2, 0, 0, 50000, G.RecordSources.Test, 0);
        private readonly RawWinLoss Lost100sNoPhase = new RawWinLoss("", SequenceName, BossName2, BossSceneName2, 0, 1, 0, 0, 0, 0, 0, 100000, G.RecordSources.Test, 0);
        private readonly RawWinLoss Lost100sPhase3 = new RawWinLoss("", SequenceName, BossName2, BossSceneName2, 0, 1, 0, 0, 0, 0, 0, 100000, G.RecordSources.Test, 3);

        private IEnumerable<TestUtils.MessageBusTestCase> GetTestCases()
        {
            return new[]
            {
                // one record
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
                    Name = "first record, lost, no phase",
                    GlobalData = new G.GlobalData { NotifyForRecord = true },
                    FolderData = new G.FolderData { RawWinLosses = new List<RawWinLoss>() },
                    InputMessages = new IMessage[]
                    {
                        Lost100sNoPhase,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        new BusEvent { ForTest = true, Event = "Lost to Absolute Radiance in HoG" },
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "first record, lost, has phase",
                    GlobalData = new G.GlobalData { NotifyForRecord = true },
                    FolderData = new G.FolderData { RawWinLosses = new List<RawWinLoss>() },
                    InputMessages = new IMessage[]
                    {
                        Lost100sPhase3,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        new BusEvent { ForTest = true, Event = "Lost to Absolute Radiance phase 3 in HoG" },
                    },
                },

                // two records
                new TestUtils.MessageBusTestCase
                {
                    Name = "2nd record, both better",
                    GlobalData = new G.GlobalData { NotifyForRecord = true },
                    FolderData = new G.FolderData { RawWinLosses = new List<RawWinLoss>() },
                    InputMessages = new IMessage[]
                    {
                        Win1Hit100s,
                        WinHitless50s,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        new BusEvent { ForTest = true, Event = "Won against The Collector in HoG (PB 1:40, 1 hit)" },
                        new BusEvent { ForTest = true, Event = "Won against The Collector in HoG (PB 0:50, hitless)" },
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "2nd record, hit better, time tie",
                    GlobalData = new G.GlobalData { NotifyForRecord = true },
                    FolderData = new G.FolderData { RawWinLosses = new List<RawWinLoss>() },
                    InputMessages = new IMessage[]
                    {
                        Win1Hit100s,
                        WinHitless100s,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        new BusEvent { ForTest = true, Event = "Won against The Collector in HoG (PB 1:40, 1 hit)" },
                        new BusEvent { ForTest = true, Event = "Won against The Collector in HoG (PB 1:40, hitless)" },
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "2nd record, time better, hit tie",
                    GlobalData = new G.GlobalData { NotifyForRecord = true },
                    FolderData = new G.FolderData { RawWinLosses = new List<RawWinLoss>() },
                    InputMessages = new IMessage[]
                    {
                        Win1Hit100s,
                        Win1Hit50s,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        new BusEvent { ForTest = true, Event = "Won against The Collector in HoG (PB 1:40, 1 hit)" },
                        new BusEvent { ForTest = true, Event = "Won against The Collector in HoG (PB 0:50, 1 hit)" },
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "2nd record, hit better, time worse",
                    GlobalData = new G.GlobalData { NotifyForRecord = true },
                    FolderData = new G.FolderData { RawWinLosses = new List<RawWinLoss>() },
                    InputMessages = new IMessage[]
                    {
                        Win1Hit100s,
                        WinHitless150s,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        new BusEvent { ForTest = true, Event = "Won against The Collector in HoG (PB 1:40, 1 hit)" },
                        new BusEvent { ForTest = true, Event = "Won against The Collector in HoG (PB hitless)" },
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "2nd record, time better, hit worse",
                    GlobalData = new G.GlobalData { NotifyForRecord = true },
                    FolderData = new G.FolderData { RawWinLosses = new List<RawWinLoss>() },
                    InputMessages = new IMessage[]
                    {
                        Win1Hit100s,
                        Win2Hits50s,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        new BusEvent { ForTest = true, Event = "Won against The Collector in HoG (PB 1:40, 1 hit)" },
                        new BusEvent { ForTest = true, Event = "Won against The Collector in HoG (PB 0:50)" },
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "2nd record, hit tie, time worse",
                    GlobalData = new G.GlobalData { NotifyForRecord = true },
                    FolderData = new G.FolderData { RawWinLosses = new List<RawWinLoss>() },
                    InputMessages = new IMessage[]
                    {
                        Win1Hit100s,
                        Win1Hit150s,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        new BusEvent { ForTest = true, Event = "Won against The Collector in HoG (PB 1:40, 1 hit)" },
                        new BusEvent { ForTest = true, Event = "Won against The Collector in HoG" },
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "2nd record, time tie, hit worse",
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
                new TestUtils.MessageBusTestCase
                {
                    Name = "2nd record, both worse",
                    GlobalData = new G.GlobalData { NotifyForRecord = true },
                    FolderData = new G.FolderData { RawWinLosses = new List<RawWinLoss>() },
                    InputMessages = new IMessage[]
                    {
                        Win1Hit100s,
                        Win2Hits150s,
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