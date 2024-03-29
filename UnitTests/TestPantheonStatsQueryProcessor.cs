using GodhomeWinLossTracker.Utils;

namespace UnitTests
{
    [TestClass]
    public class TestPantheonStatsQueryProcessor
    {
        private const string SequenceName = "P1";
        private const int PantheonIndex = 0;
        private static readonly string[] PantheonScenes = GodhomeUtils.PantheonBossSceneNames[PantheonIndex];
        private static readonly Action<string, string, string> DoNothing = (_, _, _) => { };
        private static readonly PantheonStatsQuery Query = new(PantheonIndex, GodhomeUtils.PantheonAttributes.None, DoNothing);

        private static readonly RawWinLoss[] RecordsWon = PantheonScenes
            .Zip(Enumerable.Range(0, PantheonScenes.Length))
            .Select(tuple => new RawWinLoss("", SequenceName, "", tuple.First, PantheonScenes.Length - tuple.Second, 0, 0, 0, 0, 0, 0, 0, G.RecordSources.Test, 0))
            .ToArray();
        private static readonly string ExpectedWon = "Runs: 10 | PB: Win | null";

        private static readonly RawWinLoss[] RecordsHalfwayThrough = PantheonScenes
            .Zip(Enumerable.Range(0, PantheonScenes.Length))
            .Select(tuple =>
            {
                // Wins in first segment
                if (tuple.Second < PantheonScenes.Length / 2)
                {
                    return new RawWinLoss("", SequenceName, "", tuple.First, PantheonScenes.Length - tuple.Second, 1, 0, 0, 0, 0, 0, 0, G.RecordSources.Test, 0);
                }
                // Lost in the middle
                else if (tuple.Second == PantheonScenes.Length / 2)
                {
                    return new RawWinLoss("", SequenceName, "", tuple.First, 0, 1, 0, 0, 0, 0, 0, 0, G.RecordSources.Test, 0);
                }
                // No win/loss in second segment
                else
                {
                    return new RawWinLoss("", SequenceName, "", tuple.First, 0, 0, 0, 0, 0, 0, 0, 0, G.RecordSources.Test, 0);
                }
            })
            .ToArray();
        private static readonly string ExpectedHalfwayThrough = "Runs: 11 | PB: Hornet (Protector) | Top churns: Gorb 0%, Hornet (Protector) 86%, Massive Moss Charger 88%";

        private static readonly RawWinLoss[] RecordsFailedFromStart = PantheonScenes
            .Take(1)
            .Select(scene => new RawWinLoss("", SequenceName, "", scene, 0, 4, 0, 0, 0, 0, 0, 0, G.RecordSources.Test, 0))
            .ToArray();
        private static readonly string ExpectedFailedFromStart = "Runs: 4 | null | Top churns: Vengefly King 0%";

        private static readonly RawWinLoss[] RecordsNeverAttempted = new RawWinLoss[0];

        private IEnumerable<TestUtils.MessageBusTestCase> GetTestCases()
        {
            return new[]
            {
                new TestUtils.MessageBusTestCase
                {
                    Name = "won",
                    FolderData = new G.FolderData { RawWinLosses = new(RecordsWon) },
                    InputMessages = new IMessage[] { Query },
                    ExpectedMessages = new IMessage[] {
                        Query,
                        new BusEvent { ForTest = true, Event = ExpectedWon },
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "halfway through",
                    FolderData = new G.FolderData { RawWinLosses = new(RecordsHalfwayThrough) },
                    InputMessages = new IMessage[] { Query },
                    ExpectedMessages = new IMessage[] {
                        Query,
                        new BusEvent { ForTest = true, Event = ExpectedHalfwayThrough },
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "failed from start",
                    FolderData = new G.FolderData { RawWinLosses = new(RecordsFailedFromStart) },
                    InputMessages = new IMessage[] { Query },
                    ExpectedMessages = new IMessage[] {
                        Query,
                        new BusEvent { ForTest = true, Event = ExpectedFailedFromStart },
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "never attempted",
                    FolderData = new G.FolderData { RawWinLosses = new(RecordsNeverAttempted) },
                    InputMessages = new IMessage[] { Query },
                    ExpectedMessages = new IMessage[] {
                        Query,
                        // No test event
                    },
                },
            };
        }

        [TestMethod]
        public void TestRunAll()
        {
            foreach (var testCase in GetTestCases())
            {
                testCase.HandlersCreator = _ => new[] { new PantheonStatsQueryProcessor(TestUtils.TestLocalizer) };
                testCase.InputMessages = testCase.InputMessages;
                testCase.ExpectedMessages = testCase.ExpectedMessages;
                TestUtils.TestMessageBus(testCase);
            }
        }
    }
}