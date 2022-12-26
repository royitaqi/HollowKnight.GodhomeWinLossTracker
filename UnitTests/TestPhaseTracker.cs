namespace UnitTests
{
    [TestClass]
    public class TestPhaseTracker
    {
        private const string SequenceName = "Test";
        private readonly BossChange NonBoss = new BossChange();
        private readonly BossChange AbsRad = new BossChange("AbsRad", "GG_Radiance");
        private readonly BossHpPos AbsRadPhase1 = new BossHpPos { MaxHP = 2280, HP = 2240, X = 11, Y = 22 };
        private readonly BossHpPos AbsRadPhase2 = new BossHpPos { MaxHP = 2280, HP = 1860, X = 11, Y = 22 };
        private readonly BossHpPos AbsRadPhase6 = new BossHpPos { MaxHP = 2280, HP = 200, X = 11, Y = 22 };
        private readonly RawPhase AbsRadPhase1Record = new RawPhase("Test Timestamp", SequenceName, "AbsRad", "GG_Radiance", 1, 0, 0, 0, 0, 0, GodhomeWinLossTracker.RecordSources.Test);
        private readonly RawPhase AbsRadPhase2Record = new RawPhase("Test Timestamp", SequenceName, "AbsRad", "GG_Radiance", 2, 0, 0, 0, 0, 0, GodhomeWinLossTracker.RecordSources.Test);
        private readonly RawPhase AbsRadPhase6Record = new RawPhase("Test Timestamp", SequenceName, "AbsRad", "GG_Radiance", 6, 0, 0, 0, 0, 0, GodhomeWinLossTracker.RecordSources.Test);
        private readonly BossChange VengeflyKing = new BossChange("Vengefly King", "GG_Vengefly_V");
        private readonly BossHpPos VengeflyKingPhase0 = new BossHpPos { MaxHP = 999, HP = 900, X = 11, Y = 22 };

        private IEnumerable<TestUtils.MessageBusTestCase> GetTestCases()
        {
            string testSet = "WinLossResults - ";
            return new[]
            {
                new TestUtils.MessageBusTestCase
                {
                    Name = testSet + "simply starting boss fight shouldn't generate phase record",
                    InputMessages = new IMessage[]
                    {
                        NonBoss,
                        AbsRad,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = testSet + "ending boss fight should generate phase record",
                    InputMessages = new IMessage[]
                    {
                        NonBoss,
                        AbsRad,
                        NonBoss,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        AbsRadPhase1Record,
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = testSet + "ending boss fight on a non-phased boss shouldn't generate phase record",
                    InputMessages = new IMessage[]
                    {
                        NonBoss,
                        VengeflyKing,
                        VengeflyKingPhase0,
                        NonBoss,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = testSet + "taking hit in first phase shouldn't generate phase record",
                    InputMessages = new IMessage[]
                    {
                        NonBoss,
                        AbsRad,
                        AbsRadPhase1,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = testSet + "advancing to next phase should generate phase record for previous phase",
                    InputMessages = new IMessage[]
                    {
                        NonBoss,
                        AbsRad,
                        AbsRadPhase2,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        AbsRadPhase1Record,
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = testSet + "ending boss fight after going through some phases should generate record for the last phase",
                    InputMessages = new IMessage[]
                    {
                        NonBoss,
                        AbsRad,
                        AbsRadPhase2,
                        AbsRadPhase6,
                        NonBoss,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        AbsRadPhase1Record,
                        AbsRadPhase2Record,
                        AbsRadPhase6Record,
                    },
                },
            };
        }

        [TestMethod]
        public void TestAllCases()
        {
            foreach (var testCase in GetTestCases())
            {
                testCase.HandlersCreator = _ => new[] { new PhaseTracker(() => 0) };
                testCase.InputMessages = new[] { new SequenceChange { Name = "Test" } }.Concat(testCase.InputMessages);
                testCase.OutputMessageFilter = messages => messages.Where(msg => msg is RawPhase);
                TestUtils.TestMessageBus(testCase);
            }
        }
    }
}