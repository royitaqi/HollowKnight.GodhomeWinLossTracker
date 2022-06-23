using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GodhomeWinLossTracker.MessageBus.Messages;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class WinLossGenerator : IHandler
    {
        public void OnMessage(TheMessageBus bus, Modding.Loggable logger, IMessage message)
        {
            if (message is SequenceChange)
            {
                SequenceChange msg = message as SequenceChange;
                DevUtils.Assert(msg.Name != null, "Sequence name shouldn't be null");
                _currentSequence = msg.Name;
            }
            else if (message is BossChange)
            {
                // It should never happen that there is a current boss and there is a boss change.
                // A boss fight should end either with a win or a loss. Both should have led to a reset, clearing the current boss to null.
                DevUtils.Assert(_currentBoss == null, "It should never happen that there is a current boss and there is a boss change");

                // Initialize to new boss.
                BossChange msg = message as BossChange;
                if (!msg.IsNoBoss())
                {
                    _currentBoss = msg;
                    _fightStartGameTime = DevUtils.GetTimestampEpochMs();
                    _currentKillsRequiredToWin = GodhomeUtils.GetKillsRequiredToWin(msg.SceneName);
                }
            }
            else if (message is BossDeath)
            {
                DevUtils.Assert(_currentBoss != null, "Shouldn't see boss death event when there is no current boss");
                _currentKillsRequiredToWin--;

                // Achieving the required deaths to win. Mark a win.
                if (_currentKillsRequiredToWin == 0) {
                    EmitRecord(bus, true);
                    Reset();
                }
            }
            else if (message is TKDreamDeath)
            {
                DevUtils.Assert(_currentBoss != null, "Shouldn't see boss death event when there is no current boss");

                // TK died in dream fight. Mark a loss.
                EmitRecord(bus, false);
                Reset();
            }
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
                DevUtils.GetTimestampEpochMs() - _fightStartGameTime, // fightLengthMs
                RawWinLoss.Sources.Mod
            ));
        }

        private void Reset()
        {
            _currentBoss = null;
            _fightStartGameTime = -1;
            _currentKillsRequiredToWin = -1;
        }

        // Always have the latest sequence name, even when not in a boss fight.
        private string _currentSequence = null;
        // Null value means currently no boss.
        // Non-null value means currently fighting a boss.
        private BossChange _currentBoss = null;
        private long _fightStartGameTime = -1;
        private int _currentKillsRequiredToWin = -1;
    }
}
