using System;
using System.Linq;
using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;
using Modding;
using Newtonsoft.Json;
using UnityEngine;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class Debugger : Handler
    {
        public override void Load(IGodhomeWinLossTracker mod, TheMessageBus bus, Modding.ILogger logger)
        {
            base.Load(mod, bus, logger);
            ModHooks.HeroUpdateHook += OnHeroUpdate;
            On.HeroController.Start += HeroController_Start;
            On.GameManager.Start += GameManager_Start;
        }

        private void GameManager_Start(On.GameManager.orig_Start orig, GameManager self)
        {
            _mod.LogMod($"GameManager_Start");
            orig(self);
        }

        private void HeroController_Start(On.HeroController.orig_Start orig, HeroController self)
        {
            _mod.LogMod($"HeroController_Start");
            orig(self);
        }

        private void OnHeroUpdate()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                _mod.LogMod("Debugger: 'P' pressed");
                _mod.LogMod(LoggingUtils.DumpLogCount());
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                _mod.LogMod("Debugger: 'T' pressed");
                _bus.Put(new BusEvent { ForTest = true });
            }
            else if (Input.GetKeyDown(KeyCode.LeftBracket))
            {
                _mod.LogMod("Debugger: '[' pressed");
                int idx = Array.IndexOf(_logLevels, LoggingUtils.LogLevel);
                if (idx == -1) return;

                idx = (idx + _logLevels.Length - 1) % _logLevels.Length;
                LoggingUtils.LogLevel = _logLevels[idx];
                _mod.LogMod($"LogLevel = {LoggingUtils.LogLevel}");
                ModDisplay.instance.Notify($"LogLevel = {LoggingUtils.LogLevel}");
            }
            else if (Input.GetKeyDown(KeyCode.RightBracket))
            {
                _mod.LogMod("Debugger: ']' pressed");
                int idx = Array.IndexOf(_logLevels, LoggingUtils.LogLevel);
                if (idx == -1) return;

                idx = (idx + 1) % _logLevels.Length;
                LoggingUtils.LogLevel = _logLevels[idx];
                _mod.LogMod($"LogLevel = {LoggingUtils.LogLevel}");
                ModDisplay.instance.Notify($"LogLevel = {LoggingUtils.LogLevel}");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                _mod.LogMod("Debugger: '0' pressed");

                // Print the last win/loss and hit record
                if (_mod.folderData.RawWinLosses.Count > 0)
                {
                    _mod.LogMod(JsonConvert.SerializeObject(_mod.folderData.RawWinLosses.Last()));
                }
                if (_mod.folderData.RawHits.Count > 0)
                {
                    _mod.LogMod(JsonConvert.SerializeObject(_mod.folderData.RawHits.Last()));
                }

                // Save data right now!
                _bus.Put(new SaveFolderData { Slot = GameManager.instance.profileID });
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                _mod.LogMod("Debugger: '9' pressed - loading FsmUtils");

                // Turn this line on/off to get FSM related events
                //FsmUtils.Load(this, fsm => fsm.gameObject.name == "Mage Knight" && fsm.FsmName == "Mage Knight");
                //FsmUtils.Load(this, fsm => fsm.gameObject.name == "Giant Fly" && fsm.FsmName == "Big Fly Control");
                FsmUtils.Load(_logger);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                _mod.LogMod("Debugger: '8' pressed - loading ModDisplayUtils");

                // Turn this line on/off to get ModDisplay related backdoors
                ModDisplayUtils.Initialize(); 
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                //WipeRawHitTkStats();
                //WipeRawHitTkBossPos();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
            }
        }

        private void WipeRawHitTkStats()
        {
            // Wipe all TK status in previous Hit records
            foreach (var hit in _mod.folderData.RawHits)
            {
                typeof(RawHit).GetProperty("TKStatus").SetValue(hit, 0);
            }
            _mod.LogMod("!!! All TKStatus wiped in Hit records !!!");
        }

        private void WipeRawHitTkBossPos()
        {
            // Wipe all TK and boss position info in previous Hit records
            foreach (var hit in _mod.folderData.RawHits)
            {
                typeof(RawHit).GetProperty("TKPosX").SetValue(hit, 0);
                typeof(RawHit).GetProperty("TKPosY").SetValue(hit, 0);
                typeof(RawHit).GetProperty("BossPosX").SetValue(hit, 0);
                typeof(RawHit).GetProperty("BossPosY").SetValue(hit, 0);
            }
            _mod.LogMod("!!! All TK and boss position wiped in Hit records !!!");
        }

        private LogLevel[] _logLevels = new[]
        {
            LogLevel.Fine,
            LogLevel.Debug,
            LogLevel.Info,
        };
    }
}
