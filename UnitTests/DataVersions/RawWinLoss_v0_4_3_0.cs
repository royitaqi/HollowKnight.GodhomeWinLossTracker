﻿using GodhomeWinLossTracker.Utils;

namespace UnitTests
{
    public class RawWinLoss_v0_4_3_0 : IMessage
    {
        public RawWinLoss_v0_4_3_0(string timestamp, string sequenceName, string bossName, string sceneName, int wins, int losses, int heals, int healAmount, int hits, int hitAmount, float bossHP, long fightLengthMs, RecordSources_v0_4_3_0 recordSource)
        {
            DevUtils.Assert(timestamp != null, "timestamp shouldn't be null");
            DevUtils.Assert(sequenceName != null, "sequenceName shouldn't be null");
            DevUtils.Assert(bossName != null, "bossName shouldn't be null");
            DevUtils.Assert(sceneName != null, "sceneName shouldn't be null");

            Timestamp = timestamp;
            SequenceName = sequenceName;
            BossName = bossName;
            SceneName = sceneName;
            Wins = wins;
            Losses = losses;
            Heals = heals;
            HealAmount = healAmount;
            Hits = hits;
            HitAmount = hitAmount;
            BossHP = bossHP;
            FightLengthMs = fightLengthMs;
            RecordSource = recordSource;
        }

        public override string ToString()
        {
            DevUtils.Assert(RecordSource == RecordSources_v0_4_3_0.Mod || RecordSource == RecordSources_v0_4_3_0.Test, "Only records generated by mod can be displayed");
            DevUtils.Assert(Wins == 1 && Losses == 0 || Wins == 0 && Losses == 1, "Records generated by mod should only have one win or one loss");
            string verb = Wins == 1 ? "Won" : "Lost to";
            return $"{verb} {BossName} in {SequenceName}";
        }

        public string Timestamp { get; private set; }
        public string SequenceName { get; private set; }
        public string BossName { get; private set; }
        public string SceneName { get; private set; }
        public int Wins { get; private set; }
        public int Losses { get; private set; }
        public int Heals { get; private set; }
        public int HealAmount { get; private set; }
        public int Hits { get; private set; }
        public int HitAmount { get; private set; }
        public float BossHP { get; private set; }
        public long FightLengthMs { get; private set; }
        public RecordSources_v0_4_3_0 RecordSource { get; private set; }
    }
}
