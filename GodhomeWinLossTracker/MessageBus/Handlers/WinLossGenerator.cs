﻿using System;
using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class WinLossGenerator : Handler
    {
        // Facts:
        // A. It's possible to see BossDeath event after TKDreamDeath event.
        //   * This can happen when the boss has long death animation and has minions/hazards to kill TK while the death animation is playing. One example is The Collector.
        // B. It's possible to see TKDreamDeath event after BossDeath event.
        //   * This can happen when the boss has minions/hazards to kill TK after the boss' death. One example is Gruz Mother.
        // C. It's possible to see BossDeath event after boss change.
        //   * See video and log:
        //     * https://www.youtube.com/watch?v=mjFlYRexdMM
        //     * https://www.youtube.com/watch?v=3lrYMDBB3ic
        //     * Note that, although both videos share the same event sequence, they register as different result (one win, the other loss).

        // Logic of detecting wins and losses:
        // 1. During boss fights, boss kills and TK dream deaths are memorized. No win/loss will be registered.
        // 2. All wins and losses will be registered at the end of boss fights:
        //   2.1 If there was TK dream death, register a loss.
        //   2.2 If kill count meet required count, register a win.
        //   2.3 Otherwise, register a loss (i.e. player leave fight without winning nor dying).
        //   2.4 Always: reset and/or prepare according to next boss.
        // 3. Boss kills and TK dream deaths outside boss fights will be ignored.

        public void OnSequenceChange(TheMessageBus bus, Modding.ILogger logger, SequenceChange msg)
        {
            DevUtils.Assert(msg.Name != null, "Sequence name shouldn't be null");
            _currentSequence = msg.Name;
        }

        public void OnTKDreamDeath(TheMessageBus bus, Modding.ILogger logger, TKDreamDeath msg)
        {
            if (_currentBoss != null)
            {
                // 1
                _tkDreamDeaths++;
            }
            else
            {
                // 3
            }
        }

        public void OnBossDeath(TheMessageBus bus, Modding.ILogger logger, BossDeath msg)
        {
            if (_currentBoss != null)
            {
                // 1
                _bossKills++;
            }
            else
            {
                // 3
            }
        }

        public void OnBossChange(TheMessageBus bus, Modding.ILogger logger, BossChange msg)
        {
            // 2
            // If a boss fight was on going (and hasn't been concluded yet), it's time to conclude the boss fight and register either a win or a loss.
            if (_currentBoss != null)
            {
                DevUtils.Assert(_tkDreamDeaths == 0 || _tkDreamDeaths == 1, "TK can only die zero or one time during a boss fight");
                if (_tkDreamDeaths > 0)
                {
                    // 2.1
                    EmitRecord(bus, false);
                }
                else
                {
                    int requiredBossKills = GodhomeUtils.GetKillsRequiredToWin(_currentBoss.SceneName);
                    DevUtils.Assert(_bossKills <= requiredBossKills, "Actually boss kill counts should never exceed required counts");

                    // 2.2 and 2.3
                    EmitRecord(bus, _bossKills == requiredBossKills);
                }
                // 2.4
                Reset();
            }

            // Initialize to new boss.
            if (!msg.IsNoBoss())
            {
                _currentBoss = msg;
                _tkDreamDeaths = 0;
                _bossKills = 0;
                _fightStartGameTime = DevUtils.GetTimestampEpochMs();
                _healCount = 0;
                _healAmount = 0;
                _hitCount = 0;
                _hitAmount = 0;
            }
        }

        public void OnTKHeal(TheMessageBus bus, Modding.ILogger logger, TKHeal msg)
        {
            _healCount++;
            _healAmount += msg.Heal;
        }

        public void OnTKHit(TheMessageBus bus, Modding.ILogger logger, TKHit msg)
        {
            _hitCount++;
            _hitAmount += msg.Damage;
        }

        private void EmitRecord(TheMessageBus bus, bool winLoss)
        {
            bus.Put(new RawWinLoss(
                DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss"),
                _currentSequence,
                _currentBoss.BossName,
                _currentBoss.SceneName,
                winLoss ? 1 : 0,
                winLoss ? 0 : 1,
                _healCount,
                _healAmount,
                _hitCount,
                _hitAmount,
                DevUtils.GetTimestampEpochMs() - _fightStartGameTime, // fightLengthMs
                RawWinLoss.Sources.Mod
            ));
        }

        private void Reset()
        {
            _currentBoss = null;
            _tkDreamDeaths = -1;
            _bossKills = -1;
            _fightStartGameTime = -1;
            _healCount = -1;
            _healAmount = -1;
            _hitCount = -1;
            _hitAmount = -1;
        }

        // Always have the latest sequence name, even when not in a boss fight.
        private string _currentSequence = null;
        // Null value means currently no boss.
        // Non-null value means currently fighting a boss.
        private BossChange _currentBoss = null;
        private int _tkDreamDeaths = -1;
        private int _bossKills = -1;
        private long _fightStartGameTime = -1;
        private int _healCount = -1;
        private int _healAmount = -1;
        private int _hitCount = -1;
        private int _hitAmount = -1;
    }
}
