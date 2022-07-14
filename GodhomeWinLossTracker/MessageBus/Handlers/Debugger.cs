using System;
using System.Collections.Generic;
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
                _mod.LogMod(LoggingUtils.DumpLogCount());
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                _bus.Put(new BusEvent { ForTest = true });
            }
            else if (Input.GetKeyDown(KeyCode.LeftBracket))
            {
                switch (LoggingUtils.LogLevel)
                {
                    case LogLevel.Info:
                        LoggingUtils.LogLevel = LogLevel.Debug;
                        _mod.LogModDebug("LogLevel = Debug");
                        ModDisplay.instance.Notify("LogLevel = Debug");
                        break;
                    case LogLevel.Debug:
                        LoggingUtils.LogLevel = LogLevel.Fine;
                        _mod.LogModFine("LogLevel = Fine");
                        ModDisplay.instance.Notify("LogLevel = Fine");
                        break;
                    case LogLevel.Fine:
                        LoggingUtils.LogLevel = LogLevel.Info;
                        _mod.LogMod("LogLevel = Info");
                        ModDisplay.instance.Notify("LogLevel = Info");
                        break;
                    default:
                        throw new AssertionFailedException("Should never arrive here");
                };
            }
            else if (Input.GetKeyDown(KeyCode.RightBracket))
            {
                switch (LoggingUtils.LogLevel)
                {
                    case LogLevel.Info:
                        LoggingUtils.LogLevel = LogLevel.Fine;
                        _mod.LogModFine("LogLevel = Fine");
                        ModDisplay.instance.Notify("LogLevel = Fine");
                        break;
                    case LogLevel.Fine:
                        LoggingUtils.LogLevel = LogLevel.Debug;
                        _mod.LogModDebug("LogLevel = Debug");
                        ModDisplay.instance.Notify("LogLevel = Debug");
                        break;
                    case LogLevel.Debug:
                        LoggingUtils.LogLevel = LogLevel.Info;
                        _mod.LogMod("LogLevel = Info");
                        ModDisplay.instance.Notify("LogLevel = Info");
                        break;
                    default:
                        throw new AssertionFailedException("Should never arrive here");
                };
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                _mod.LogMod(JsonConvert.SerializeObject(_mod.folderData));
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
            }
        }
    }
}
