using System;
using System.Collections.Generic;
using GodhomeWinLossTracker.MessageBus.Messages;
using UnityEngine;

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

    internal class BossHpPosUpdater : Handler
    {
        public void OnBossChange(BossChange msg)
        {
            _bossGOs.Clear();
            _isInFight = msg.IsBoss();
            _maxHP = 0;
        }

        public void OnEnemyEnabled(EnemyEnabled msg)
        {
            if (_isInFight && msg.IsBoss)
            {
                _bossGOs.Add(msg.EnemyGO);
                _maxHP += msg.EnemyGO.GetComponent<HealthManager>().hp;

                // Send boss hp and pos when boss is enabled
                SendBossHpPos();
            }
        }

        // Send boss hp and pos when any enemy is damaged
        public void OnEnemyDamaged(EnemyDamaged _)
        {
            SendBossHpPos();
        }

        private void SendBossHpPos()
        {
            if (!_isInFight)
            {
                return;
            }

            var msg = new BossHpPos
            {
                MaxHP = _maxHP,
                HP = 0,
                X = 0,
                Y = 0,
            };

            int count = 0;
            foreach (var boss in _bossGOs)
            {
                // Bosses killed during a fight can become null
                if (boss == null)
                {
                    continue;
                }

                HealthManager hm = boss.GetComponent<HealthManager>();
                if (hm.isDead)
                {
                    continue;
                }

                msg.X += boss.transform.position.x;
                msg.Y += boss.transform.position.y;
                msg.HP += Math.Max(0, hm.hp);
                count++;
            }
            msg.HP = Math.Min(msg.HP, msg.MaxHP);
            if (count != 0)
            {
                msg.X /= count;
                msg.Y /= count;
            }

            _bus.Put(msg);
        }

        private readonly List<GameObject> _bossGOs = new();
        private bool _isInFight = false;
        private int _maxHP = 0;
    }
}
