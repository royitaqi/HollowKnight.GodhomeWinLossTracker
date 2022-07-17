namespace UnitTests
{
    [TestClass]
    public class TestFightTracker
    {
        private const string SequenceName = "Test";
        private const string BossName = "Hornet (Protector)";
        private const string BossSceneName = "GG_Hornet_1";
        private const string BossName2 = "Hornet (Sentinel)";
        private const string BossSceneName2 = "GG_Hornet_2";
        private readonly BossChange NonBoss = new BossChange();
        private readonly BossChange Boss = new BossChange(BossName, BossSceneName);
        private readonly BossChange Boss2 = new BossChange(BossName2, BossSceneName2);
        private readonly RawWinLoss Win = new RawWinLoss("", SequenceName, BossName, BossSceneName, 1, 0, 0, 0, 0, 0, 0, 0, G.RecordSources.Test);
        private readonly RawWinLoss Loss = new RawWinLoss("", SequenceName, BossName, BossSceneName, 0, 1, 0, 0, 0, 0, 0, 0, G.RecordSources.Test);
        private readonly BossDeath BossKill = new BossDeath();
        private readonly TKDreamDeath TKDeath = new TKDreamDeath();

        private IEnumerable<TestUtils.MessageBusTestCase> GetTestCases()
        {
            return new[]
            {
                new TestUtils.MessageBusTestCase
                {
                    Name = "non-boss => nothing",
                    InputMessages = new IMessage[]
                    {
                        NonBoss,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        NonBoss,
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "boss => nothing",
                    InputMessages = new IMessage[]
                    {
                        Boss,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        Boss,
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "non-boss > non-boss => nothing",
                    InputMessages = new IMessage[]
                    {
                        NonBoss,
                        NonBoss,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        NonBoss,
                        NonBoss,
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "non-boss > boss => nothing",
                    InputMessages = new IMessage[]
                    {
                        NonBoss,
                        Boss,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        NonBoss,
                        Boss,
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "boss > non-boss => loss",
                    InputMessages = new IMessage[]
                    {
                        Boss,
                        NonBoss,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        Boss,
                        NonBoss,
                        Loss,
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "boss > boss => loss",
                    InputMessages = new IMessage[]
                    {
                        Boss,
                        Boss2,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        Boss,
                        Boss2,
                        Loss,
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "non-boss > tk death => nothing",
                    InputMessages = new IMessage[]
                    {
                        NonBoss,
                        TKDeath,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        NonBoss,
                        TKDeath,
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "non-boss > boss kill => nothing",
                    InputMessages = new IMessage[]
                    {
                        NonBoss,
                        BossKill,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        NonBoss,
                        BossKill,
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "boss > tk death => nothing",
                    InputMessages = new IMessage[]
                    {
                        Boss,
                        TKDeath,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        Boss,
                        TKDeath,
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "boss > boss kill => nothing",
                    InputMessages = new IMessage[]
                    {
                        Boss,
                        BossKill,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        Boss,
                        BossKill,
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "boss > tk death > boss kill > non-boss => loss",
                    InputMessages = new IMessage[]
                    {
                        Boss,
                        TKDeath,
                        BossKill,
                        NonBoss,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        Boss,
                        TKDeath,
                        BossKill,
                        NonBoss,
                        Loss,
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "boss > tk death > boss kill > boss => loss",
                    InputMessages = new IMessage[]
                    {
                        Boss,
                        TKDeath,
                        BossKill,
                        Boss2,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        Boss,
                        TKDeath,
                        BossKill,
                        Boss2,
                        Loss,
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "boss > boss kill > tk death > non-boss => loss",
                    InputMessages = new IMessage[]
                    {
                        Boss,
                        BossKill,
                        TKDeath,
                        NonBoss,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        Boss,
                        BossKill,
                        TKDeath,
                        NonBoss,
                        Loss,
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "boss > boss kill > tk death > boss => loss",
                    InputMessages = new IMessage[]
                    {
                        Boss,
                        BossKill,
                        TKDeath,
                        Boss2,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        Boss,
                        BossKill,
                        TKDeath,
                        Boss2,
                        Loss,
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "boss > boss kill > non-boss > tk death => win",
                    InputMessages = new IMessage[]
                    {
                        Boss,
                        BossKill,
                        NonBoss,
                        TKDeath,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        Boss,
                        BossKill,
                        NonBoss,
                        Win,
                        TKDeath,
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "boss > boss kill > boss > tk death => win",
                    InputMessages = new IMessage[]
                    {
                        Boss,
                        BossKill,
                        Boss2,
                        TKDeath,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        Boss,
                        BossKill,
                        Boss2,
                        Win,
                        TKDeath,
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "boss > tk death > tk death > non-boss => exception",
                    InputMessages = new IMessage[]
                    {
                        Boss,
                        TKDeath,
                        TKDeath,
                        NonBoss,
                    },
                    ExpectedException = new G.Utils.AssertionFailedException("TK can only die zero or one time during a boss fight"),
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = "boss > boss kill > boss kill > non-boss => exception",
                    InputMessages = new IMessage[]
                    {
                        Boss,
                        BossKill,
                        BossKill,
                        NonBoss,
                    },
                    ExpectedException = new G.Utils.AssertionFailedException("Actually boss kill counts should never exceed required counts"),
                },
            };
        }

        [TestMethod]
        public void TestRunAll()
        {
            foreach (var testCase in GetTestCases())
            {
                testCase.HandlersCreator = _ => new[] { new FightTracker(() => 0) };
                testCase.InputMessages = new[] { new SequenceChange { Name = "Test" } }.Concat(testCase.InputMessages);
                if (testCase.ExpectedMessages != null)
                {
                    testCase.ExpectedMessages = new[] { new SequenceChange { Name = "Test" } }.Concat(testCase.ExpectedMessages);
                }
                TestUtils.TestMessageBus(testCase);
            }
        }
    }
}