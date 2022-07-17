﻿using GodhomeWinLossTracker.MessageBus.Messages;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class TKHealthObserver : Handler
    {
        public override void Load(IGodhomeWinLossTracker mod, TheMessageBus bus, Modding.ILogger logger)
        {
            base.Load(mod, bus, logger);
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
                // First send out TKHit to trigger updates, then send out RawHitRequest to create a record.
                _bus.Put(new TKHit { Damage = damage, HealthAfter = healthAfter });
                _bus.Put(new RawHitRequest());
            }
        }
    }
}
