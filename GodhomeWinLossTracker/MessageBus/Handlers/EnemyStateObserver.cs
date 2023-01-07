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

                if (Debugger.ForceBossAttacks)
                {
                    if (enemy.name == "HK Prime")
                    {
                        fsm.GetState("Choice P1").InsertMethod(0, () =>
                        {
                            fsm.SendEvent("DASH");
                        });
                    }
                    if (enemy.name == "White Defender")
                    {
                        fsm.GetState("Move Choice").InsertMethod(0, () =>
                        {
                            fsm.SetState("Roll Speed");
                        });
                        fsm.GetState("Air Dive?").AddMethod(() =>
                        {
                            fsm.FsmVariables.GetFsmInt("Bounces").Value = 9999;
                        });
                    }
                }
            }

            // Set up damage source hooks
            var damageSourceFsms = GodhomeUtils.GetBossDamageSourceFsms(enemy.name);
            if (damageSourceFsms != null)
            {
                _logger.LogModFine($"EnemyStateObserver:   Damage source FSMs found for enemy: {String.Join(",", damageSourceFsms.Select(dsf => dsf.DamageSource + "-" + dsf.DamageSourceDetail))}");
                foreach (var dsf in damageSourceFsms)
                {
                    var lambda = () =>
                    {
                        var damageSourceGo = dsf.VariableName != null
                            ? fsm.FsmVariables.GetFsmGameObject(dsf.VariableName).Value
                            : enemy;
                        _logger.LogModFine($"EnemyStateObserver: Damage source {damageSourceGo.name} spawned");

                        damageSourceGo.SetGoTag(dsf.DamageSource, dsf.DamageSourceDetail);

                        _logger.LogModFine($"EnemyStateObserver: Damage source {damageSourceGo.name} tagged with \"{dsf.DamageSource}-{dsf.DamageSourceDetail}\"");
                    };

                    var fsmState = enemy
                        .LocateMyFSM(dsf.FsmName)
                        .GetState(dsf.StateName);

                    // Pick an index for the new action. Priority is:
                    // 1) If dsf.ActionIndex is specified, use it.
                    // 2) If dsf.VariableName is not specified, use 0 (because it doesn't have to wait for the variable object to be initialized by one of the actions.
                    // 3) After all existing actions.
                    int? index = dsf.ActionIndex ?? (dsf.VariableName == null ? 0 : null);
                    if (index == null)
                    {
                        fsmState.AddMethod(lambda);
                    } else
                    {
                        fsmState.InsertMethod(index.Value, lambda);
                    }
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
