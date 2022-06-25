using Modding;

namespace UnitTests
{
    [TestClass]
    public class TestWinLossGenerator
    {
        private const string SequenceName = "Test";
        private const string BossName = "Hornet (Sentinel)";
        private const string BossSceneName = "GG_Hornet_2";
        private readonly BossChange NonBoss = new BossChange();
        private readonly BossChange Boss = new BossChange(BossName, BossSceneName);

        private IEnumerable<TestUtils.MessageBusTestCase> GetTestCases()
        {
            return new[]
            {
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
                        new RawWinLoss("", SequenceName, BossName, BossSceneName, 0, 1, 0, RawWinLoss.Sources.Mod),
                    },
                },
            };
        }

        [TestMethod]
        public void TestRunAll()
        {
            var handlers = new[] { new WinLossGenerator() };

            foreach (var testCase in GetTestCases())
            {
                testCase.Handlers = handlers;
                testCase.InputMessages = new[] { new SequenceChange { Name = "Test" } }.Concat(testCase.InputMessages);
                testCase.ExpectedMessages = new[] { new SequenceChange { Name = "Test" } }.Concat(testCase.ExpectedMessages);
                TestUtils.TestMessageBus(testCase);
            }
        }
    }
}