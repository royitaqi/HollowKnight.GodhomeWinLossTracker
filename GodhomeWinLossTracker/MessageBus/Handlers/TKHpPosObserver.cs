using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class TKHpPosObserver : Handler
    {
        public override void Load(IGodhomeWinLossTracker mod, TheMessageBus bus, Modding.ILogger logger)
        {
            base.Load(mod, bus, logger);
            On.PlayerData.AddHealth += PlayerData_AddHealth;
            On.HeroController.TakeDamage += HeroController_TakeDamage;
        }

        public override void Unload()
        {
            base.Unload();
            On.PlayerData.AddHealth -= PlayerData_AddHealth;
            On.HeroController.TakeDamage -= HeroController_TakeDamage;
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

        private void HeroController_TakeDamage(On.HeroController.orig_TakeDamage orig, HeroController self, UnityEngine.GameObject go, GlobalEnums.CollisionSide damageSide, int damageAmount, int hazardType)
        {
            int healthBefore = PlayerData.instance.health + PlayerData.instance.healthBlue;
            orig(self, go, damageSide, damageAmount, hazardType);
            int healthAfter = PlayerData.instance.health + PlayerData.instance.healthBlue;
            int damage = healthBefore - healthAfter;

            if (damage != 0)
            {
                var tag = go.GetGoTag();
                string damageSource = null;
                string damageSourceDetail = null;

                if (tag?.DamageSource != null)
                {
                    // Has tag. Just use the tag.
                    damageSource = tag.DamageSource;
                    damageSourceDetail = tag.DamageSourceDetail;
                }
                else
                {
                    // Put mapped string as damage source and Go's name as detail.
                    damageSource = GodhomeUtils.MapDamageSource(go);
                    damageSourceDetail = go.name;
                }


                _bus.Put(new TKHit {
                    Damage = damage,
                    HealthAfter = healthAfter,
                    DamageSource = damageSource,
                    DamageSourceDetail = damageSourceDetail,
                });
            }
        }

        public void OnTKHpPosRequest(TKHpPosRequest msg)
        {
            _bus.Put(new TKHpPos {
                HP = PlayerData.instance.health + PlayerData.instance.healthBlue,
                X = HeroController.instance.gameObject.transform.position.x,
                Y = HeroController.instance.gameObject.transform.position.y,
            });
        }
    }
}
