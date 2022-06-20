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
                    bus.Put(new FightWinLoss { SequenceName = _currentSequence, BossName = _currentBoss, WinLoss = false });
                }

                // Initialize to new boss.
                BossChange msg = message as BossChange;
                if (msg.IsNoBoss())
                {
                    _currentBoss = null;
                    _currentKillsRequiredToWin = -1;
                }
                else
                {
                    _currentBoss = msg.BossName;
                    _currentKillsRequiredToWin = GodhomeUtils.GetKillsRequiredToWin(msg.SceneName);
                }
            }
            else if (message is BossDeath)
            {
                DevUtils.Assert(_currentBoss != null, "Shouldn't see boss death event when there is no current boss");
                _currentKillsRequiredToWin--;

                // Achieving the required deaths to win. Mark a win.
                if (_currentKillsRequiredToWin == 0) {
                    bus.Put(new FightWinLoss { SequenceName = _currentSequence, BossName = _currentBoss, WinLoss = true });

                    _currentBoss = null;
                    _currentKillsRequiredToWin = -1;
                }
            }
        }

        private string _currentSequence = null;
        private string _currentBoss = null;
        private int _currentKillsRequiredToWin = -1;
    }
}
