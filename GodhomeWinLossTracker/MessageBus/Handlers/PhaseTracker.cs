using System;
using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class PhaseTracker : Handler
    {
        public PhaseTracker(Func<long> getGameTime)
        {
            _getGameTime = getGameTime;
        }

        public override void Unload()
        {
            base.Unload();
            Reset();
            _currentSequence = null;
        }

        public void OnSequenceChange(SequenceChange msg)
        {
            DevUtils.Assert(msg.Name != null, "Sequence name shouldn't be null");
            _currentSequence = msg.Name;
        }

        public void OnBossChange(BossChange msg)
        {
            // Emit record when boss changes
            if (_currentBoss != null)
            {
                EmitRawPhase();
                Reset();
            }

            if (msg.IsBoss())
            {
                int phase = GodhomeUtils.GetBossInitialPhase(msg.SceneName);
                Init(msg, phase);
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
        }

        public void OnBossHpPos(BossHpPos msg)
        {
            int phase = GodhomeUtils.GetBossPhase(_currentBoss.SceneName, msg.MaxHP, msg.HP);

            // Emit record when phase changes
            if (phase != _phase)
            {
                EmitRawPhase();
                Init(_currentBoss, phase);
            }
        }

        private void EmitRawPhase()
        {
            DevUtils.Assert(_phaseStartGameTime != -1, "Phase start time should have been initialized");

            // Don't emit phase 0 - they simply mean no phase.
            if (_phase == 0)
            {
                return;
            }

            _bus.Put(new RawPhase(
                DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss"),
                _currentSequence,
                _currentBoss.BossName,
                _currentBoss.SceneName,
                _phase,
                _healCount,
                _healAmount,
                _hitCount,
                _hitAmount,
                _getGameTime() - _phaseStartGameTime,
                RecordSources.Mod
            ));
        }

        private void Reset()
        {
            _currentBoss = null;
            _phase = -1;
            _healCount = -1;
            _healAmount = -1;
            _hitCount = -1;
            _hitAmount = -1;
            _phaseStartGameTime = -1;
        }

        private void Init(BossChange newBoss, int phase)
        {
            _logger.LogModDebug($"Entering phase {phase}");
            _currentBoss = newBoss;
            _phase = phase;
            _healCount = 0;
            _healAmount = 0;
            _hitCount = 0;
            _hitAmount = 0;
            _phaseStartGameTime = _getGameTime();
        }

        Func<long> _getGameTime;
        // Always have the latest sequence name, even when not in a boss fight.
        private string _currentSequence = null;
        // Null value means currently no boss.
        // Non-null value means currently fighting a boss.
        private BossChange _currentBoss = null;
        private int _phase = -1;
        private int _healCount = -1;
        private int _healAmount = -1;
        private int _hitCount = -1;
        private int _hitAmount = -1;
        private long _phaseStartGameTime = -1;
    }
}
