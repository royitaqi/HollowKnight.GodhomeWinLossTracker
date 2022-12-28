using System;
using System.Linq;
using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;
using Modding;
using UnityEngine;
using Vasi;

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

        public override void Unload()
        {
            base.Unload();
            ModHooks.OnEnableEnemyHook -= ModHooks_OnEnableEnemyHook;
            On.HealthManager.TakeDamage -= HealthManager_TakeDamage;
        }

        private void HealthManager_TakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            orig(self, hitInstance);
            _bus.Put(new EnemyDamaged());
        }

        private bool ModHooks_OnEnableEnemyHook(GameObject enemy, bool isAlreadyDead)
        {
            _logger.LogModFine($"EnemyStateObserver: Setting up FSM for enemy {enemy.name}");
            var isBoss = IsBoss(enemy);

            // Set up things with the boss' FSM
            var fsm = isBoss ? SelectFSM(enemy) : null;
            if (fsm != null)
            {
                _logger.LogModFine($"EnemyStateObserver:   FSM found for enemy: {fsm.FsmName}");
                FsmUtils.HookAllStates(fsm, state =>
                {
                    _bus.Put(new BossStateChange { BossGO = enemy, FSM = fsm, State = state });
                });

                _logger.LogModTEMP($"Boss FSM name = {fsm.name}");
                if (fsm.name == "Mage Knight")
                {
                    fsm
                        .GetState("Shoot")
                        .AddMethod(() =>
                        {
                            var orb = fsm.FsmVariables.GetFsmGameObject("Orb").Value;
                            _logger.LogModTEMP($"Orb name = {orb.name}");
                            var tag = orb.AddComponent<GoTag>();
                            _logger.LogModTEMP($"tag == null? {tag == null}");
                            if (tag != null)
                            {
                                tag.DamageSource = "Shoot";
                            }
                        });
                }
            }

            // Set up damage source hooks
            var damageSourceFsms = GodhomeUtils.GetBossDamageSourceFsms(enemy.name);
            if (damageSourceFsms != null)
            {
                _logger.LogModFine($"EnemyStateObserver:   Damage source FSMs found for enemy: {String.Join(",", damageSourceFsms.Select(dsf => dsf.DamageSource + "-" + dsf.DamageSourceDetail))}");
                foreach (var dsf in damageSourceFsms)
                {
                    enemy
                        .LocateMyFSM(dsf.FsmName)
                        .GetState(dsf.StateName)
                        .AddMethod(() =>
                        {
                            var damageSourceGo = fsm.FsmVariables.GetFsmGameObject(dsf.VariableName).Value;
                            _logger.LogModFine($"EnemyStateObserver: Damage source {damageSourceGo.name} spawned");

                            damageSourceGo.SetGoTag(dsf.DamageSource, dsf.DamageSourceDetail);

                            //// Tag the damage source GO
                            //GoTag tag = null;
                            //if (!damageSourceGo.TryGetComponent<GoTag>(out tag))
                            //{
                            //    tag = damageSourceGo.AddComponent<GoTag>();
                            //}
                            //DevUtils.Assert(tag != null, $"Should be able to find/add a GoTag for damage source {damageSourceGo.name}");
                            //tag.DamageSource = ;
                            //tag.DamageSourceDetail = ;

                            //// Override existing tags in children GO
                            //var tags = damageSourceGo.GetComponentsInChildren<GoTag>(true);
                            //foreach (var t in tags)
                            //{
                            //    t.DamageSource = dsf.DamageSource;
                            //    t.DamageSourceDetail = dsf.DamageSourceDetail;
                            //}

                            _logger.LogModFine($"EnemyStateObserver: Damage source {damageSourceGo.name} tagged with \"{dsf.DamageSource}-{dsf.DamageSourceDetail}\"");
                        });
                }
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
            // Check if an FSM has been pre-decided for the boss.
            var fsmName = GodhomeUtils.GetBossFsm(enemy.name);
            if (fsmName != null)
            {
                return enemy.LocateMyFSM(fsmName);
            }

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

            _logger.LogModWarn($"Cannot find FSM for boss {enemy.name}");
            return null;
        }
    }
}
