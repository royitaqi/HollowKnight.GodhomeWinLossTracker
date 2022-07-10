using System;
using System.Collections.Generic;
using System.Linq;
using GodhomeWinLossTracker.MessageBus.Messages;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    // Special cases:
    // - The Collector:
    //   - Fighting: 900/900
    //   - Death anim: -50/-50
    // - Oro & Mato:
    //   - Starting:
    //     - Mato = 1000/1000
    //   - 1st phase:
    //     - Oro = 500/500
    //     - Mato = 1000/1000, invis
    //   - Intermission:
    //     - Oro = -50/500
    //     - Mato = 1000/1000
    //   - 2nd phase
    //     - Oro = 600/500
    //     - Mato = 1000/1000
    //   - Death anim
    //     - Oro = -50/500
    //     - Mato = -1/1000
    // - God Tamer:
    //   - Lobster (God) = 750
    //   - Lancer (Tamer) = 750, not critical
    // - Sisters of Battle:
    //   - 1st phase:
    //     - Mantis Lord = 500 \
    //     - Mantis Lord = 500 / these two are connected
    //   - 2nd phase:
    //     - Mantis Lord S1 = 750 \
    //     - Mantis Lord S1 = 750 /
    //     - Mantis Lord S2 = 750 \
    //     - Mantis Lord S2 = 750 /
    //     - Mantis Lord S3 = 750 \
    //     - Mantis Lord S3 = 750 /
    // - AbsRad
    //   - 1st phase: 2280/2280
    //   - 2nd phase: 1880/2280
    //   - 3rd phase: 1430/2280
    //   - 4th phase: 1130/2280
    //   - 5th phase: 380/2280
    //   (reset to 280 no matter the health)
    //   - 6th phase: 280/2280

    internal class BossHPUpdater : Handler
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
                HealthManager hm = msg.EnemyGO.GetComponent<HealthManager>();
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
                bus.Put(new BossHP { MaxHP = _maxHP, HP = Math.Min(_maxHP, _healthManagers.Select(hm => GetHP(hm)).Sum()) });
            }
        }

        private int GetHP(HealthManager hm)
        {
            return hm.isDead ? 0 : Math.Max(0, hm.hp);
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
