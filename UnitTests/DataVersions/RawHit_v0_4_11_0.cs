﻿using GodhomeWinLossTracker.Utils;

namespace UnitTests
{
    public class RawHit_v0_4_11_0 : IMessage
    {
        public RawHit_v0_4_11_0(string timestamp, string sequenceName, string bossName, string sceneName, int tkStatus, int tkPosX, int tkPosY, int tkHealthBefore, int damageAmount, DamageSources_v0_4_11_0 damageSource, float bossHP, string bossState, int bossPosX, int bossPosY, long fightLengthMs, RecordSources_v0_4_11_0 recordSource)
        {
            DevUtils.Assert(timestamp!= null, "timestamp shouldn't be null");
            DevUtils.Assert(sequenceName != null, "sequenceName shouldn't be null");
            DevUtils.Assert(bossName != null, "bossName shouldn't be null");
            DevUtils.Assert(sceneName != null, "sceneName shouldn't be null");

            Timestamp = timestamp;
            SequenceName = sequenceName;
            BossName = bossName;
            SceneName = sceneName;
            TKStatus = tkStatus;
            TKPosX = tkPosX;
            TKPosY = tkPosY;
            TKHealthBefore = tkHealthBefore;
            DamageAmount = damageAmount;
            DamageSource = damageSource;
            BossHP = bossHP;
            BossState = bossState;
            BossPosX = bossPosX;
            BossPosY = bossPosY;
            FightLengthMs = fightLengthMs;
            RecordSource = recordSource;
        }

        public override string ToString()
        {
            DevUtils.Assert(RecordSource == RecordSources_v0_4_11_0.Mod || RecordSource == RecordSources_v0_4_11_0.Test, "Only records generated by mod can be displayed");
            return $"TK hit by {BossName} in {SequenceName} at {TKHealthBefore} masks and {(int)(BossHP * 100)}% boss HP";
        }

        public string Timestamp { get; private set; }
        public string SequenceName { get; private set; }
        public string BossName { get; private set; }
        public string SceneName { get; private set; }
        public int TKStatus { get; private set; }
        public int TKPosX { get; private set; }
        public int TKPosY { get; private set; }
        public int TKHealthBefore { get; private set; }
        public int DamageAmount { get; private set; }
        public DamageSources_v0_4_11_0 DamageSource { get; private set; }
        public float BossHP { get; private set; }
        public string BossState { get; private set; }
        public int BossPosX { get; private set; }
        public int BossPosY { get; private set; }
        public long FightLengthMs { get; private set; } // The amount of time into the fight (not finish time)
        public RecordSources_v0_4_11_0 RecordSource { get; private set; }
    }
}
