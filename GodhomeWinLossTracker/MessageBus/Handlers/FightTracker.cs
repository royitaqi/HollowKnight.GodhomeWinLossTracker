using System;
using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class FightTracker : Handler
    {
        public FightTracker(Func<long> getGameTime)
        {
            _getGameTime = getGameTime;
        }

        public override void Unload()
        {
            base.Unload();
            Reset();
            _currentSequence = null;
        }

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

        public void OnSequenceChange(SequenceChange msg)
        {
            DevUtils.Assert(msg.Name != null, "Sequence name shouldn't be null");
            _currentSequence = msg.Name;
        }

        public void OnTKDreamDeath(TKDreamDeath msg)
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

        public void OnBossDeath(BossDeath msg)
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

        public void OnBossChange(BossChange msg)
        {
            // 2
            // If a boss fight was on going (and hasn't been concluded yet), it's time to conclude the boss fight and register either a win or a loss.
            if (_currentBoss != null)
            {
                if (_tkDreamDeaths > 0)
                {
                    // 2.1
                    // Coding defensively here that if the TK death count is greater than 1, we will still consider it a "lose".
                    if (_tkDreamDeaths > 1)
                    {
                        _logger.LogModWarn($"TK dream death count ({_tkDreamDeaths}) should only be 0 or 1");
                    }
                    EmitRawWinLoss(false);
                }
                else
                {
                    // 2.2 and 2.3
                    // Coding defensively here that if the boss kill count is greater than the required kills, we will still consider it a "win".
                    int requiredBossKills = GodhomeUtils.GetKillsRequiredToWin(_currentBoss.SceneName);
                    if (_bossKills > requiredBossKills)
                    {
                        _logger.LogModWarn($"Actually boss kill count ({_bossKills}) should not exceed required count ({requiredBossKills})");
                    }
                    EmitRawWinLoss(_bossKills >= requiredBossKills);
                }
                // 2.4
                Reset();
            }

            // Initialize to new boss.
            if (msg.IsBoss())
            {
                Init(msg);
            }
        }

        public void OnTKHeal(TKHeal msg)
        {
            _healCount++;
            _healAmount += msg.Heal;
        }

        public void OnTKHit(TKHit msg)
        {
            _hitCount++;
            _hitAmount += msg.Damage;
            _lastTKHit = msg;

            // Poll for boss HP and pos info
            _bus.Put(new TKHpPosRequest());
        }

        public void OnTKHpPos(TKHpPos msg)
        {
            _bus.Put(new RawHit(
                DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss"),
                _currentSequence,
                _currentBoss.BossName,
                _currentBoss.SceneName,
                _tkStatus,
                (int)Math.Round(msg.X),
                (int)Math.Round(msg.Y),
                _lastTKHit.HealthBefore,
                _lastTKHit.Damage,
                _lastTKHit.DamageSource,
                _lastTKHit.DamageSourceDetail,
                _lastBossHpPos.MaxHP != 0 ? (float)_lastBossHpPos.HP / _lastBossHpPos.MaxHP : 1,
                _bossState,
                (int)Math.Round(_lastBossHpPos.X),
                (int)Math.Round(_lastBossHpPos.Y),
                _getGameTime() - _fightStartGameTime,
                RecordSources.Mod,
                GodhomeUtils.GetBossPhase(_currentBoss.SceneName, _lastBossHpPos.MaxHP, _lastBossHpPos.HP)
            ));
        }

        public void OnTKStatus(TKStatus msg)
        {
            _tkStatus = msg.Status;
        }

        public void OnBossHpPos(BossHpPos msg)
        {
            _lastBossHpPos = msg;
        }

        public void OnBossStateChange(BossStateChange msg)
        {
            _bossState = msg.State.Name;
        }

        private void EmitRawWinLoss(bool winLoss)
        {
            _bus.Put(new RawWinLoss(
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
                _lastBossHpPos.MaxHP != 0 ? (float)_lastBossHpPos.HP / _lastBossHpPos.MaxHP : 1,
                _getGameTime() - _fightStartGameTime,
                RecordSources.Mod,
                GodhomeUtils.GetBossPhase(_currentBoss.SceneName, _lastBossHpPos.MaxHP, _lastBossHpPos.HP)
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
            _bossState = null;
            _lastTKHit = null;
            _lastBossHpPos = null;
            _tkStatus = -1;
        }

        private void Init(BossChange newBoss)
        {
            _currentBoss = newBoss;
            _tkDreamDeaths = 0;
            _bossKills = 0;
            _fightStartGameTime = _getGameTime();
            _healCount = 0;
            _healAmount = 0;
            _hitCount = 0;
            _hitAmount = 0;
            _bossState = "N/A";
            _lastTKHit = null; // This will be populated before emitting a RawHit record
            _lastBossHpPos = null; // This will be populated before emitting a RawHit record
            _tkStatus = -1;
        }

        Func<long> _getGameTime;
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
        private string _bossState = null;
        private TKHit _lastTKHit = null;
        private BossHpPos _lastBossHpPos = null;
        private int _tkStatus = -1;
    }
}
