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
        private const string BossState = "N/A"; // This is the hardcoded value in FightTracker as initial value for boss state.
        private readonly BossChange NonBoss = new BossChange();
        private readonly BossChange Boss = new BossChange(BossName, BossSceneName);
        private readonly BossChange Boss2 = new BossChange(BossName2, BossSceneName2);
        private readonly BossHpPos BossHpPos100 = new BossHpPos { MaxHP = 100, HP = 100, X = 11, Y = 22 };
        private readonly BossHpPos BossHpPos80 = new BossHpPos { MaxHP = 100, HP = 80, X = 11, Y = 22 };
        private readonly BossHpPos BossHpPos60 = new BossHpPos { MaxHP = 100, HP = 60, X = 11, Y = 22 };
        private readonly BossHpPos BossHpPos40 = new BossHpPos { MaxHP = 100, HP = 40, X = 11, Y = 22 };
        private readonly BossHpPos BossHpPos20 = new BossHpPos { MaxHP = 100, HP = 20, X = 11, Y = 22 };
        private readonly BossHpPos BossHpPos0 = new BossHpPos { MaxHP = 100, HP = 0, X = 11, Y = 22 };
        private readonly RawWinLoss Win = new RawWinLoss("", SequenceName, BossName, BossSceneName, 1, 0, 0, 0, 0, 0, 0.8f, 0, G.RecordSources.Test, 0);
        private readonly RawWinLoss Loss = new RawWinLoss("", SequenceName, BossName, BossSceneName, 0, 1, 0, 0, 0, 0, 0.8f, 0, G.RecordSources.Test, 0);
        private readonly BossDeath BossKill = new BossDeath();
        private readonly TKDreamDeath TKDeath = new TKDreamDeath();
        private readonly TKStatus TKStatus4 = new TKStatus { Status = 8375 };
        private readonly TKStatus TKStatus3 = new TKStatus { Status = 6523 };
        private readonly TKStatus TKStatus2 = new TKStatus { Status = 4034 };
        private readonly TKHit TKHit4To3 = new TKHit { Damage = 1, HealthAfter = 3 };
        private readonly TKHit TKHit3To2 = new TKHit { Damage = 1, HealthAfter = 2 };
        private readonly TKHit TKHit2To1 = new TKHit { Damage = 1, HealthAfter = 1 };
        private readonly TKHit TKHit1To0 = new TKHit { Damage = 1, HealthAfter = 0 };
        private readonly TKHpPos TKHpPos4 = new TKHpPos { HP = 4, X = 1, Y = 2 };
        private readonly TKHpPos TKHpPos3 = new TKHpPos { HP = 3, X = 1, Y = 2 };
        private readonly TKHpPos TKHpPos2 = new TKHpPos { HP = 2, X = 1, Y = 2 };
        private readonly TKHpPos TKHpPos1 = new TKHpPos { HP = 1, X = 1, Y = 2 };

        private IEnumerable<TestUtils.MessageBusTestCase> GetTestCasesWinLossResults()
        {
            string testSet = "WinLossResults - ";
            return new[]
            {
                new TestUtils.MessageBusTestCase
                {
                    Name = testSet + "non-boss => nothing",
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
                    Name = testSet + "boss => nothing",
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
                    Name = testSet + "non-boss > non-boss => nothing",
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
                    Name = testSet + "non-boss > boss => nothing",
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
                    Name = testSet + "boss > non-boss => loss",
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
                    Name = testSet + "boss > boss => loss",
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
                    Name = testSet + "non-boss > tk death => nothing",
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
                    Name = testSet + "non-boss > boss kill => nothing",
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
                    Name = testSet + "boss > tk death => nothing",
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
                    Name = testSet + "boss > boss kill => nothing",
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
                    Name = testSet + "boss > tk death > boss kill > non-boss => loss",
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
                    Name = testSet + "boss > tk death > boss kill > boss => loss",
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
                    Name = testSet + "boss > boss kill > tk death > non-boss => loss",
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
                    Name = testSet + "boss > boss kill > tk death > boss => loss",
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
                    Name = testSet + "boss > boss kill > non-boss > tk death => win",
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
                    Name = testSet + "boss > boss kill > boss > tk death => win",
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
                    Name = testSet + "boss > tk death > tk death > non-boss => loss",
                    InputMessages = new IMessage[]
                    {
                        Boss,
                        TKDeath,
                        TKDeath,
                        NonBoss,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        Boss,
                        TKDeath,
                        TKDeath,
                        NonBoss,
                        Loss,
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = testSet + "boss > boss kill > boss kill > non-boss => win",
                    InputMessages = new IMessage[]
                    {
                        Boss,
                        BossKill,
                        BossKill,
                        NonBoss,
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        Boss,
                        BossKill,
                        BossKill,
                        NonBoss,
                        Win,
                    },
                },
            };
        }

        [TestMethod]
        public void TestWinLossResults()
        {
            foreach (var testCase in GetTestCasesWinLossResults())
            {
                testCase.HandlersCreator = _ => new[] { new FightTracker(() => 0) };
                testCase.InputMessages = new[] { new SequenceChange { Name = "Test" } }.Concat(testCase.InputMessages);
                if (testCase.ExpectedMessages != null)
                {
                    testCase.ExpectedMessages = new[] { new SequenceChange { Name = "Test" } }.Concat(testCase.ExpectedMessages);
                }

                // Insert one BossHpPos message after each Boss message
                testCase.InputMessages = InsertBossHpPosMessages(testCase.InputMessages);
                if (testCase.ExpectedMessages != null)
                {
                    testCase.ExpectedMessages = InsertBossHpPosMessages(testCase.ExpectedMessages);
                }

                TestUtils.TestMessageBus(testCase);
            }
        }

        private IEnumerable<TestUtils.MessageBusTestCase> GetTestCasesTKHits()
        {
            string testSet = "TKHits - ";
            return new[]
            {
                new TestUtils.MessageBusTestCase
                {
                    Name = testSet + "boss hp updated between 3 hits",
                    InputMessages = new IMessage[]
                    {
                        Boss,
                        BossHpPos100, // From BossHpPosUpdater
                        TKStatus4,
                        TKHit4To3,
                        TKHpPos3, // From TKHpPosObserver
                        // Should RawHit
                        BossHpPos80, // From BossHpPosUpdater
                        TKStatus3,
                        TKHit3To2,
                        TKHpPos2, // From TKHpPosObserver
                        // Should RawHit
                        BossHpPos60, // From BossHpPosUpdater
                        TKStatus2,
                        TKHit2To1,
                        TKHpPos1, // From TKHpPosObserver
                        // Should RawHit
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        new RawHit(
                            "",
                            SequenceName,
                            BossName,
                            BossSceneName,
                            TKStatus4.Status,
                            (int)Math.Round(TKHpPos3.X),
                            (int)Math.Round(TKHpPos3.Y),
                            TKHit4To3.HealthBefore,
                            TKHit4To3.Damage,
                            TKHit4To3.DamageSource,
                            null,
                            1,
                            BossState,
                            (int)Math.Round(BossHpPos100.X),
                            (int)Math.Round(BossHpPos100.Y),
                            0,
                            GodhomeWinLossTracker.RecordSources.Test,
                            0
                        ),
                        new RawHit(
                            "",
                            SequenceName,
                            BossName,
                            BossSceneName,
                            TKStatus3.Status,
                            (int)Math.Round(TKHpPos2.X),
                            (int)Math.Round(TKHpPos2.Y),
                            TKHit3To2.HealthBefore,
                            TKHit3To2.Damage,
                            TKHit3To2.DamageSource,
                            null,
                            0.8f,
                            BossState,
                            (int)Math.Round(BossHpPos80.X),
                            (int)Math.Round(BossHpPos80.Y),
                            0,
                            GodhomeWinLossTracker.RecordSources.Test,
                            0
                        ),
                        new RawHit(
                            "",
                            SequenceName,
                            BossName,
                            BossSceneName,
                            TKStatus2.Status,
                            (int)Math.Round(TKHpPos1.X),
                            (int)Math.Round(TKHpPos1.Y),
                            TKHit2To1.HealthBefore,
                            TKHit2To1.Damage,
                            TKHit2To1.DamageSource,
                            null,
                            0.6f,
                            BossState,
                            (int)Math.Round(BossHpPos60.X),
                            (int)Math.Round(BossHpPos60.Y),
                            0,
                            GodhomeWinLossTracker.RecordSources.Test,
                            0
                        ),
                    },
                },
                new TestUtils.MessageBusTestCase
                {
                    Name = testSet + "no boss hp updated between 2 hits",
                    InputMessages = new IMessage[]
                    {
                        Boss,
                        BossHpPos100, // From BossHpPosUpdater
                        TKStatus4,
                        TKHit4To3,
                        TKHpPos3, // From TKHpPosObserver
                        // Should RawHit
                        TKStatus3,
                        TKHit3To2,
                        TKHpPos2, // From TKHpPosObserver
                        // Should RawHit
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        new RawHit(
                            "",
                            SequenceName,
                            BossName,
                            BossSceneName,
                            TKStatus4.Status,
                            (int)Math.Round(TKHpPos3.X),
                            (int)Math.Round(TKHpPos3.Y),
                            TKHit4To3.HealthBefore,
                            TKHit4To3.Damage,
                            TKHit4To3.DamageSource,
                            null,
                            1,
                            BossState,
                            (int)Math.Round(BossHpPos100.X),
                            (int)Math.Round(BossHpPos100.Y),
                            0,
                            GodhomeWinLossTracker.RecordSources.Test,
                            0
                        ),
                        new RawHit(
                            "",
                            SequenceName,
                            BossName,
                            BossSceneName,
                            TKStatus3.Status,
                            (int)Math.Round(TKHpPos2.X),
                            (int)Math.Round(TKHpPos2.Y),
                            TKHit3To2.HealthBefore,
                            TKHit3To2.Damage,
                            TKHit3To2.DamageSource,
                            null,
                            1,
                            BossState,
                            (int)Math.Round(BossHpPos100.X),
                            (int)Math.Round(BossHpPos100.Y),
                            0,
                            GodhomeWinLossTracker.RecordSources.Test,
                            0
                        ),
                    },
                },
            };
        }

        [TestMethod]
        public void TestTKHits()
        {
            foreach (var testCase in GetTestCasesTKHits())
            {
                testCase.HandlersCreator = _ => new[] { new FightTracker(() => 0) };
                testCase.InputMessages = new[] { new SequenceChange { Name = "Test" } }.Concat(testCase.InputMessages);
                testCase.OutputMessageFilter = msgs => msgs.Where(msg => msg is RawHit);

                TestUtils.TestMessageBus(testCase);
            }
        }

        private IEnumerable<TestUtils.MessageBusTestCase> GetTestCasesBossPhase()
        {
            string SequenceName = "Test";
            string BossName = "Absoluate Radiance";
            string BossSceneName = "GG_Radiance";
            string BossState = "N/A"; // This is the hardcoded value in FightTracker as initial value for boss state.
            BossChange NonBoss = new BossChange();
            BossChange Boss = new BossChange(BossName, BossSceneName);
            int MaxHP = 2280;
            int Phase = 3;
            int PhaseHP = 1280;
            BossHpPos BossHpPos = new BossHpPos { MaxHP = MaxHP, HP = PhaseHP, X = 11, Y = 22 };
            BossDeath BossKill = new BossDeath();
            TKDreamDeath TKDeath = new TKDreamDeath();
            TKStatus TKStatus4 = new TKStatus { Status = 8375 };
            TKHit TKHit4To3 = new TKHit { Damage = 1, HealthAfter = 3 };
            TKHpPos TKHpPos3 = new TKHpPos { HP = 3, X = 1, Y = 2 };

            string testSet = "BossPhase - ";
            return new[]
            {
                new TestUtils.MessageBusTestCase
                {
                    Name = testSet + "TK dies to AbsRad phase 3",
                    InputMessages = new IMessage[]
                    {
                        Boss,
                        BossHpPos, // From BossHpPosUpdater
                        TKStatus4,
                        TKHit4To3,
                        TKHpPos3, // From TKHpPosObserver
                        // Should RawHit
                        TKDeath,
                        NonBoss,
                        // Should RawWinLoss
                    },
                    ExpectedMessages = new IMessage[]
                    {
                        new RawHit(
                            "",
                            SequenceName,
                            BossName,
                            BossSceneName,
                            TKStatus4.Status,
                            (int)Math.Round(TKHpPos3.X),
                            (int)Math.Round(TKHpPos3.Y),
                            TKHit4To3.HealthBefore,
                            TKHit4To3.Damage,
                            TKHit4To3.DamageSource,
                            null,
                            (float)PhaseHP / MaxHP,
                            BossState,
                            (int)Math.Round(BossHpPos.X),
                            (int)Math.Round(BossHpPos.Y),
                            0,
                            GodhomeWinLossTracker.RecordSources.Test,
                            Phase
                        ),
                        new RawWinLoss(
                            "",
                            SequenceName,
                            BossName,
                            BossSceneName,
                            0,
                            1,
                            0,
                            0,
                            1,
                            1,
                            (float)PhaseHP / MaxHP,
                            0,
                            GodhomeWinLossTracker.RecordSources.Test,
                            Phase
                        ),
                    },
                },
            };
        }

        [TestMethod]
        public void TestBossPhase()
        {
            foreach (var testCase in GetTestCasesBossPhase())
            {
                testCase.HandlersCreator = _ => new[] { new FightTracker(() => 0) };
                testCase.InputMessages = new[] { new SequenceChange { Name = "Test" } }.Concat(testCase.InputMessages);
                testCase.OutputMessageFilter = msgs => msgs.Where(msg => msg is RawHit || msg is RawWinLoss);

                TestUtils.TestMessageBus(testCase);
            }
        }

        private IEnumerable<IMessage> InsertBossHpPosMessages(IEnumerable<IMessage> input)
        {
            List<IMessage> ret = new List<IMessage>();
            foreach (var msg in input)
            {
                ret.Add(msg);
                if (msg == Boss)
                {
                    ret.Add(BossHpPos80);
                }
            }
            return ret.ToArray();
        }
    }
}