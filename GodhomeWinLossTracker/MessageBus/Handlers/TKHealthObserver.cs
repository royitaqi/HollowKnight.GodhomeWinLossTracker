using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class TKHealthObserver : Handler
    {
        public override void Load(IGodhomeWinLossTracker mod, TheMessageBus bus)
        {
            base.Load(mod, bus);
            On.PlayerData.TakeHealth += PlayerData_TakeHealth;
            On.PlayerData.AddHealth += PlayerData_AddHealth;
        }

        private void PlayerData_AddHealth(On.PlayerData.orig_AddHealth orig, PlayerData self, int amount)
        {
            int healthBefore = PlayerData.instance.health + PlayerData.instance.healthBlue;
            orig(self, amount);
            int healthAfter = PlayerData.instance.health + PlayerData.instance.healthBlue;
            int heal = healthAfter - healthBefore;

            if (heal != 0)
            {
                _bus.Put(new TKHeal { Heal = heal, HealthAfter = healthAfter });
            }
        }

        private void PlayerData_TakeHealth(On.PlayerData.orig_TakeHealth orig, PlayerData self, int amount)
        {
            int healthBefore = PlayerData.instance.health + PlayerData.instance.healthBlue;
            orig(self, amount);
            int healthAfter = PlayerData.instance.health + PlayerData.instance.healthBlue;
            int damage = healthBefore - healthAfter;

            if (damage != 0)
            {
                _bus.Put(new TKHit { Damage = damage, HealthAfter = healthAfter });
            }
        }
    }
}
