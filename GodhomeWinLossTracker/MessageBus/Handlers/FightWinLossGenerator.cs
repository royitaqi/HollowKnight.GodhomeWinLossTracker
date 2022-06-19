using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GodhomeWinLossTracker.MessageBus.Messages;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class FightWinLossGenerator : IHandler
    {
        public void OnMessage(TheMessageBus bus, Modding.Loggable logger, IMessage msg)
        {
            if (msg is BossChange)
            {
                // If there is a current boss, it means it hasn't met its required number of deaths.
                // A boss change in this case means that the current boss isn't fully finished, hence a loss.
                if (_currentBoss != null)
                {
                    bus.Put(new FightWinLoss { BossName = _currentBoss, WinLoss = false });
                }

                // Initialize to new boss.
                _currentBoss = (msg as BossChange).Name;
                _currentKillsRequiredToWin = GetKillsRequiredToWin(_currentBoss);
            }
            else if (msg is BossDeath)
            {
                Debug.Assert(_currentBoss == null);
                _currentKillsRequiredToWin--;

                // Achieving the required deaths to win. Mark a win.
                if (_currentKillsRequiredToWin == 0) {
                    bus.Put(new FightWinLoss { BossName = _currentBoss, WinLoss = true });

                    _currentBoss = null;
                    _currentKillsRequiredToWin = -1;
                }
            }
        }

        private int GetKillsRequiredToWin(string bossName)
        {
            if (bossName == null)
            {
                return -1;
            }
            else if (KillsRequiredToWin.ContainsKey(bossName))
            {
                return KillsRequiredToWin[bossName];
            }
            else
            {
                return 1;
            }
        }

        private static readonly Dictionary<string, int> KillsRequiredToWin = new Dictionary<string, int>
        {
            {"Nailmasters", 2 },
        };
        private string _currentBoss = null;
        private int _currentKillsRequiredToWin = -1;
    }
}
