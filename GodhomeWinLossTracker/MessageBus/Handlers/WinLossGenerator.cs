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
                // If there is a current boss, it means it hasn't met its required number of kills.
                // A boss change in this case means that the current boss isn't fully finished, hence a loss.
                if (_currentBoss != null)
                {
                    EmitRecord(bus, false);
                }

                // Initialize to new boss.
                BossChange msg = message as BossChange;
                if (msg.IsNoBoss())
                {
                    Reset();
                }
                else
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
