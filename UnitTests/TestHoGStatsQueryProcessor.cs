using Modding;

namespace UnitTests
{
    [TestClass]
    public class TestHoGStatsQueryProcessor
    {
        private const string SequenceName = "HoG";
        private const string IrrelevantSequenceName = "P1";
        private static readonly Action<string> DoNothing = _ => { };


        private const string BossA = "Soul Warrior";
        private const string BossANameKey = "NAME_MAGE_KNIGHT";
        private const string BossAAttunedScene = "GG_Mage_Knight";
        private const string BossAAscendedScene = "GG_Mage_Knight_V";

        private const string BossB = "Lost Kin";
        private const string BossBNameKey = "NAME_LOST_KIN";
        private const string BossBScene = "GG_Lost_Kin";

        private static readonly HoGStatsQuery QueryTwoSceneBothStats = new(BossANameKey, DoNothing);
        private static readonly RawWinLoss[] RecordsTwoSceneBothStats = new[]
        {
            new RawWinLoss("", SequenceName, BossA, BossAAscendedScene, 10, 5, 0, RawWinLoss.Sources.Manual),
            // Irellevant sequence
            new RawWinLoss("", IrrelevantSequenceName, BossA, BossAAscendedScene, 10, 5, 0, RawWinLoss.Sources.Manual),
            // Reverted order of scenes
            new RawWinLoss("", SequenceName, BossA, BossAAttunedScene, 10, 5, 0, RawWinLoss.Sources.Manual),
            new RawWinLoss("", IrrelevantSequenceName, BossA, BossAAttunedScene, 10, 5, 0, RawWinLoss.Sources.Manual),
            // Irrelevant boss
            new RawWinLoss("", SequenceName, BossB, BossBScene, 1000, 2000, 0, RawWinLoss.Sources.Manual),
            // More stats
            new RawWinLoss("", SequenceName, BossA, BossAAttunedScene, 4, 1, 0, RawWinLoss.Sources.Manual),
            new RawWinLoss("", IrrelevantSequenceName, BossA, BossAAttunedScene, 4, 1, 0, RawWinLoss.Sources.Manual),
            new RawWinLoss("", SequenceName, BossA, BossAAscendedScene, 20, 45, 0, RawWinLoss.Sources.Manual),
            new RawWinLoss("", IrrelevantSequenceName, BossA, BossAAscendedScene, 20, 45, 0, RawWinLoss.Sources.Manual),
        };
        private static readonly string ExpectedTwoSceneBothStats = "Attuned: 20 fights, 14 wins (70%)\r\nAscended+: 80 fights, 30 wins (38%)\r\n";

        private static readonly HoGStatsQuery QueryTwoSceneOnlyAttunedStats = new(BossANameKey, DoNothing);
        private static readonly RawWinLoss[] RecordsTwoSceneOnlyAttunedStats = new[]
        {
            new RawWinLoss("", SequenceName, BossA, BossAAttunedScene, 10, 5, 0, RawWinLoss.Sources.Manual),
            // Irrelevant boss
            new RawWinLoss("", SequenceName, BossB, BossBScene, 1000, 2000, 0, RawWinLoss.Sources.Manual),
            // More stats
            new RawWinLoss("", SequenceName, BossA, BossAAttunedScene, 4, 1, 0, RawWinLoss.Sources.Manual),
        };
        private static readonly string ExpectedTwoSceneOnlyAttunedStats = "Attuned: 20 fights, 14 wins (70%)\r\n";

        private static readonly HoGStatsQuery QueryTwoSceneOnlyAscendedStats = new(BossANameKey, DoNothing);
        private static readonly RawWinLoss[] RecordsTwoSceneOnlyAscendedStats = new[]
        {
            new RawWinLoss("", SequenceName, BossA, BossAAscendedScene, 10, 5, 0, RawWinLoss.Sources.Manual),
            // Irrelevant boss
            new RawWinLoss("", SequenceName, BossB, BossBScene, 1000, 2000, 0, RawWinLoss.Sources.Manual),
            // More stats
            new RawWinLoss("", SequenceName, BossA, BossAAscendedScene, 20, 45, 0, RawWinLoss.Sources.Manual),
        };
        private static readonly string ExpectedTwoSceneOnlyAscendedStats = "Ascended+: 80 fights, 30 wins (38%)\r\n";

        private static readonly HoGStatsQuery QueryTwoSceneNoStats = new(BossANameKey, DoNothing);
        private static readonly RawWinLoss[] RecordsTwoSceneNoStats = new[]
        {
            // Irrelevant boss
            new RawWinLoss("", SequenceName, BossB, BossBScene, 1000, 2000, 0, RawWinLoss.Sources.Manual),
        };
        private static readonly string ExpectedTwoSceneNoStats = "null";

        private static readonly HoGStatsQuery QueryOneSceneSomeStats = new(BossBNameKey, DoNothing);
        private static readonly RawWinLoss[] RecordsOneSceneSomeStats = new[]
        {
            new RawWinLoss("", SequenceName, BossB, BossBScene, 10, 5, 0, RawWinLoss.Sources.Manual),
            // Irrelevant boss
            new RawWinLoss("", SequenceName, BossA, BossAAscendedScene, 1000, 2000, 0, RawWinLoss.Sources.Manual),
            // More stats
            new RawWinLoss("", SequenceName, BossB, BossBScene, 20, 45, 0, RawWinLoss.Sources.Manual),
        };
        private static readonly string ExpectedOneSceneSomeStats = "80 fights, 30 wins (38%)\r\n";

        private static readonly HoGStatsQuery QueryOneSceneNoStats = new(BossBNameKey, DoNothing);
        private static readonly RawWinLoss[] RecordsOneSceneNoStats = new[]
        {
            // Irrelevant boss
            new RawWinLoss("", SequenceName, BossA, BossAAttunedScene, 1000, 2000, 0, RawWinLoss.Sources.Manual),
            // Irellevant sequence
            new RawWinLoss("", IrrelevantSequenceName, BossB, BossBScene, 10, 5, 0, RawWinLoss.Sources.Manual),
        };
        private static readonly string ExpectedOneSceneNoStats = "null";

        private IEnumerable<TestUtils.MessageBusTestCase> GetTestCases()
        {
            return new[]
            {
                new TestUtils.MessageBusTestCase
                {
                    Name = "TwoSceneBothStats",
                    FolderData = new G.FolderData { RawRecords = new(RecordsTwoSceneBothStats) },
                    InputMessages = new IMessage[] { QueryTwoSceneBothStats },
                    ExpectedMessages = new IMessage[] {
                        QueryTwoSceneBothStats,
                        new BusEvent { ForTest = true, Event = ExpectedTwoSceneBothStats },
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "TwoSceneOnlyAttunedStats",
                    FolderData = new G.FolderData { RawRecords = new(RecordsTwoSceneOnlyAttunedStats) },
                    InputMessages = new IMessage[] { QueryTwoSceneOnlyAttunedStats },
                    ExpectedMessages = new IMessage[] {
                        QueryTwoSceneOnlyAttunedStats,
                        new BusEvent { ForTest = true, Event = ExpectedTwoSceneOnlyAttunedStats },
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "TwoSceneOnlyAscendedStats",
                    FolderData = new G.FolderData { RawRecords = new(RecordsTwoSceneOnlyAscendedStats) },
                    InputMessages = new IMessage[] { QueryTwoSceneOnlyAscendedStats },
                    ExpectedMessages = new IMessage[] {
                        QueryTwoSceneOnlyAscendedStats,
                        new BusEvent { ForTest = true, Event = ExpectedTwoSceneOnlyAscendedStats },
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "TwoSceneNoStats",
                    FolderData = new G.FolderData { RawRecords = new(RecordsTwoSceneNoStats) },
                    InputMessages = new IMessage[] { QueryTwoSceneNoStats },
                    ExpectedMessages = new IMessage[] {
                        QueryTwoSceneNoStats,
                        new BusEvent { ForTest = true, Event = ExpectedTwoSceneNoStats },
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "OneSceneSomeStats",
                    FolderData = new G.FolderData { RawRecords = new(RecordsOneSceneSomeStats) },
                    InputMessages = new IMessage[] { QueryOneSceneSomeStats },
                    ExpectedMessages = new IMessage[] {
                        QueryOneSceneSomeStats,
                        new BusEvent { ForTest = true, Event = ExpectedOneSceneSomeStats },
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "OneSceneNoStats",
                    FolderData = new G.FolderData { RawRecords = new(RecordsOneSceneNoStats) },
                    InputMessages = new IMessage[] { QueryOneSceneNoStats },
                    ExpectedMessages = new IMessage[] {
                        QueryOneSceneNoStats,
                        new BusEvent { ForTest = true, Event = ExpectedOneSceneNoStats },
                    },
                },
            };
        }

        [TestMethod]
        public void TestRunAll()
        {
            foreach (var testCase in GetTestCases())
            {
                testCase.HandlersCreator = mod => new[] { new HoGStatsQueryProcessor(mod) };
                testCase.InputMessages = testCase.InputMessages;
                testCase.ExpectedMessages = testCase.ExpectedMessages;
                TestUtils.TestMessageBus(testCase);
            }
        }
    }
}