using System;
using System.Collections.Generic;
using System.Linq;
using GodhomeWinLossTracker.MessageBus.Messages;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class BossHPObserver : Handler
    {
        public void OnBossChange(TheMessageBus bus, Modding.ILogger logger, BossChange msg)
        {
            _healthManagers.Clear();
            _isInFight = msg.IsBoss();
            _maxHP = 0;
        }

        public void OnEnemyEnabled(TheMessageBus bus, Modding.ILogger logger, EnemyEnabled msg)
        {
            if (_isInFight)
            {
                HealthManager hm = msg.Enemy.GetComponent<HealthManager>();
                // The magic number 200 is borrowed from EnemyHPBar::Instance_OnEnableEnemyHook.
                if (!hm.isDead && IsBoss(hm))
                {
                    _healthManagers.Add(hm);
                    _maxHP += hm.hp;
                }
            }
        }

        public void OnEnemyDamaged(TheMessageBus bus, Modding.ILogger logger, EnemyDamaged msg)
        {
            if (_isInFight)
            {
                bus.Put(new BossHP { MaxHP = _maxHP, HP = _healthManagers.Select(hm => GetHP(hm)).Sum() });
            }
        }

        private int GetHP(HealthManager hm)
        {
            return hm.isDead ? 0 : Math.Max(hm.hp, 0);
        }

        private bool IsBoss(HealthManager hm)
        {
            return hm.hp >= 200;
        }

        private readonly List<HealthManager> _healthManagers = new();
        private bool _isInFight = false;
        private int _maxHP = 0;
    }
}
