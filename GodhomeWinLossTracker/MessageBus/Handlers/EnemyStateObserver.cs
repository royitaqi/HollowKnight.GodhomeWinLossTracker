using GodhomeWinLossTracker.MessageBus.Messages;
using Modding;
using UnityEngine;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class EnemyStateObserver : Handler
    {
        public override void Load(IGodhomeWinLossTracker mod, TheMessageBus bus)
        {
            base.Load(mod, bus);
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
            _bus.Put(new EnemyEnabled { Enemy = enemy });
            return isAlreadyDead;
        }
    }
}
