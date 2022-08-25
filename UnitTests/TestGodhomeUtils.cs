namespace UnitTests
{
    [TestClass]
    public class TestGodhomeUtils
    {
        struct TestCase
        {
            public string Name;
            public string BossScene;
            public int MaxHP;
            public int HP;
            public int ExpectedPhase;
        };

        private IEnumerable<TestCase> GetTestCases()
        {
            return new[]
            {
                new TestCase
                {
                    Name = "boss scene not in map",
                    BossScene = "foo",
                    MaxHP = 2280,
                    HP = 2280,
                    ExpectedPhase = 0,
                },
                new TestCase
                {
                    Name = "boss scene in map but max hp doesn't match",
                    BossScene = "GG_Radiance",
                    MaxHP = 9999,
                    HP = 2280,
                    ExpectedPhase = 0,
                },
                new TestCase
                {
                    Name = "AbsRad phase 1 upper bound",
                    BossScene = "GG_Radiance",
                    MaxHP = 2280,
                    HP = 2280,
                    ExpectedPhase = 1,
                },
                new TestCase
                {
                    Name = "AbsRad phase 1 lower bound",
                    BossScene = "GG_Radiance",
                    MaxHP = 2280,
                    HP = 1880 + 1,
                    ExpectedPhase = 1,
                },
                new TestCase
                {
                    Name = "AbsRad phase 2 upper bound",
                    BossScene = "GG_Radiance",
                    MaxHP = 2280,
                    HP = 1880,
                    ExpectedPhase = 2,
                },
                new TestCase
                {
                    Name = "AbsRad phase 2 lower bound",
                    BossScene = "GG_Radiance",
                    MaxHP = 2280,
                    HP = 1430 + 1,
                    ExpectedPhase = 2,
                },
                new TestCase
                {
                    Name = "AbsRad phase 3 upper bound",
                    BossScene = "GG_Radiance",
                    MaxHP = 2280,
                    HP = 1430,
                    ExpectedPhase = 3,
                },
                new TestCase
                {
                    Name = "AbsRad phase 3 lower bound",
                    BossScene = "GG_Radiance",
                    MaxHP = 2280,
                    HP = 1130 + 1,
                    ExpectedPhase = 3,
                },
                new TestCase
                {
                    Name = "AbsRad phase 4 upper bound",
                    BossScene = "GG_Radiance",
                    MaxHP = 2280,
                    HP = 1130,
                    ExpectedPhase = 4,
                },
                new TestCase
                {
                    Name = "AbsRad phase 4 lower bound",
                    BossScene = "GG_Radiance",
                    MaxHP = 2280,
                    HP = 380 + 1,
                    ExpectedPhase = 4,
                },
                new TestCase
                {
                    Name = "AbsRad phase 5 upper bound",
                    BossScene = "GG_Radiance",
                    MaxHP = 2280,
                    HP = 380,
                    ExpectedPhase = 5,
                },
                new TestCase
                {
                    Name = "AbsRad phase 5 lower bound",
                    BossScene = "GG_Radiance",
                    MaxHP = 2280,
                    HP = 280 + 1,
                    ExpectedPhase = 5,
                },
                new TestCase
                {
                    Name = "AbsRad phase 6 upper bound",
                    BossScene = "GG_Radiance",
                    MaxHP = 2280,
                    HP = 280,
                    ExpectedPhase = 6,
                },
                new TestCase
                {
                    Name = "AbsRad phase 6 lower bound",
                    BossScene = "GG_Radiance",
                    MaxHP = 2280,
                    HP = -999,
                    ExpectedPhase = 6,
                },
            };
        }

        [TestMethod]
        public void TestRunAll()
        {
            foreach (var testCase in GetTestCases())
            {
                int actualPhase = G.Utils.GodhomeUtils.GetBossPhase(testCase.BossScene, testCase.MaxHP, testCase.HP);
                Assert.AreEqual(testCase.ExpectedPhase, actualPhase, testCase.Name);
            }
        }
    }
}