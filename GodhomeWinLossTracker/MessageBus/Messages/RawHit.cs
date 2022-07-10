﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    public class RawHit : IMessage
    {
        public RawHit(string timestamp, string sequenceName, string bossName, string sceneName, int tkStatus, int tkHealthBefore, int damageAmount, TKHit.DamageSources damageSource, float bossHP, long fightLengthMs, RecordSources recordSource)
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
            TKHealthBefore = tkHealthBefore;
            DamageAmount = damageAmount;
            DamageSource = damageSource;
            BossHP = bossHP;
            FightLengthMs = fightLengthMs;
            RecordSource = recordSource;
        }

        public override string ToString()
        {
            DevUtils.Assert(RecordSource == RecordSources.Mod || RecordSource == RecordSources.Test, "Only records generated by mod can be displayed");
            return $"TK hit by {BossName} in {SequenceName} at {TKHealthBefore} masks and {(int)(BossHP * 100)}% boss HP";
        }

        public string Timestamp { get; private set; }
        public string SequenceName { get; private set; }
        public string BossName { get; private set; }
        public string SceneName { get; private set; }
        public int TKStatus { get; private set; }
        public int TKHealthBefore { get; private set; }
        public int DamageAmount { get; private set; }
        public TKHit.DamageSources DamageSource { get; private set; }
        public float BossHP { get; private set; }
        public long FightLengthMs { get; private set; } // The amount of time into the fight (not finish time)
        public RecordSources RecordSource { get; private set; }
    }
}
