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
        public void OnBossChange(BossChange msg)
        {
            _healthManagers.Clear();
            _isInFight = msg.IsBoss();
            _maxHP = 0;
        }

        public void OnEnemyEnabled(EnemyEnabled msg)
        {
            if (_isInFight && msg.IsBoss)
            {
                HealthManager hm = msg.EnemyGO.GetComponent<HealthManager>();
                _healthManagers.Add(hm);
                _maxHP += hm.hp;
            }
        }

        public void OnEnemyDamaged(EnemyDamaged msg)
        {
            if (_isInFight)
            {
                _bus.Put(new BossHP { MaxHP = _maxHP, HP = Math.Min(_maxHP, _healthManagers.Select(hm => GetHP(hm)).Sum()) });
            }
        }

        private int GetHP(HealthManager hm)
        {
            return hm.isDead ? 0 : Math.Max(0, hm.hp);
        }

        private readonly List<HealthManager> _healthManagers = new();
        private bool _isInFight = false;
        private int _maxHP = 0;
    }
}
