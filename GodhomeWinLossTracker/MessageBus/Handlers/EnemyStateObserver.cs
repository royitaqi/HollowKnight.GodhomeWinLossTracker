using System.Linq;
using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;
using Modding;
using UnityEngine;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class EnemyStateObserver : Handler
    {
        public override void Load(IGodhomeWinLossTracker mod, TheMessageBus bus, Modding.ILogger logger)
        {
            base.Load(mod, bus, logger);
            ModHooks.OnEnableEnemyHook += ModHooks_OnEnableEnemyHook;
            On.HealthManager.TakeDamage += HealthManager_TakeDamage;
        }

        private void HealthManager_TakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            orig(self, hitInstance);
            _bus.Put(new EnemyDamaged());
        }

        private bool ModHooks_OnEnableEnemyHook(GameObject enemy, bool isAlreadyDead)
        {
            var isBoss = IsBoss(enemy);
            var fsm = isBoss ? SelectFSM(enemy) : null;

            if (fsm != null)
            {
                FsmUtils.HookAllStates(fsm, state =>
                {
                    _bus.Put(new BossStateChange { BossGO = enemy, FSM = fsm, State = state });
                });
            }

            _bus.Put(new EnemyEnabled { EnemyGO = enemy, IsBoss = isBoss, FSM = fsm });
            return isAlreadyDead;
        }

        private bool IsBoss(GameObject enemy)
        {
            var hm = enemy.GetComponent<HealthManager>();
            // The magic number 200 is borrowed from EnemyHPBar::Instance_OnEnableEnemyHook.
            return !hm.isDead && hm.hp >= 200;
        }

        private PlayMakerFSM SelectFSM(GameObject enemy)
        {
            // Priority of selection:
            // 1. The only FSM
            // 2. The FSM with name "BOSS_NAME Control"
            // 3. The FSM with name "BOSS_NAME"
            // 4. The FSM with name ".* Control"
            var fsms = enemy.GetComponents<PlayMakerFSM>();

            if (fsms.Length == 1)
            {
                return fsms[0];
            }

            var fsm = fsms.FirstOrDefault(fsm => fsm.name == enemy.name + " Control");
            if (fsm != null)
            {
                return fsm;
            }

            fsm = fsms.FirstOrDefault(fsm => fsm.name == enemy.name);
            if (fsm != null)
            {
                return fsm;
            }

            fsm = fsms.FirstOrDefault(fsm => fsm.name.EndsWith(" Control"));
            if (fsm != null)
            {
                return fsm;
            }

            return null;
        }
    }
}
