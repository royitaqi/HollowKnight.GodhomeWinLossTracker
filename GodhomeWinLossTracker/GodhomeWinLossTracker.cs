using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Modding;
using UnityEngine;
using GodhomeWinLossTracker.MessageBus;
using GodhomeWinLossTracker.MessageBus.Messages;
using Newtonsoft.Json;

namespace GodhomeWinLossTracker
{
    public class GodhomeWinLossTracker : Mod, IGlobalSettings<GlobalData>, ILocalSettings<LocalData>, ICustomMenuMod, ITogglableMod
    {
        internal static GodhomeWinLossTracker instance;
        private TheMessageBus messageBus;
        public GlobalData globalData = new GlobalData();
        internal LocalData localData = new LocalData();

        ///
        /// Mod
        ///

        public override string GetVersion() => "0.0.7.0";
        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
#if DEBUG
            Log("Initializing mod");
#endif

            instance = this;

            messageBus = new();
            messageBus.Initialize(this);

            // Production hooks
            ModHooks.BeforeSceneLoadHook += OnSceneLoad;
            On.BossSceneController.EndBossScene += OnEndBossScene;

#if DEBUG
            // Debug hooks
            ModHooks.HeroUpdateHook += OnHeroUpdate;
            ModHooks.AfterPlayerDeadHook += OnPlayerDeath;

            On.BossSceneController.CheckBossesDead += BossSceneController_CheckBossesDead;
            //On.BossSceneController.EndSceneDelayed += BossSceneController_EndSceneDelayed;
#endif

            ModDisplay.Initialize();

#if DEBUG
            Log("Initialized");
#endif
        }

        //private IEnumerator BossSceneController_EndSceneDelayed(On.BossSceneController.orig_EndSceneDelayed orig, BossSceneController self)
        //{
        //    messageBus.Put(new BusEvent { Event = "EndSceneDelayed enter" });
        //    yield return new WaitForSeconds(5);
        //    messageBus.Put(new BusEvent { Event = "EndSceneDelayed leave" });
        //}

        private void BossSceneController_CheckBossesDead(On.BossSceneController.orig_CheckBossesDead orig, BossSceneController self)
        {
            messageBus.Put(new BusEvent { Event = "CheckBossesDead before" });
            orig(self);
            messageBus.Put(new BusEvent { Event = "CheckBossesDead end" });
        }

        ///
        /// ICustomMenuMod
        ///

        public bool ToggleButtonInsideMenu => true;
        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggle) => ModMenu.GetMenu(modListMenu, toggle);

        ///
        /// ITogglableMod
        ///
        public void Unload()
        {
#if DEBUG
            Log("Unloading");
#endif

            // Production hooks
            ModHooks.BeforeSceneLoadHook -= OnSceneLoad;
            On.BossSceneController.EndBossScene -= OnEndBossScene;

#if DEBUG
            // Debug hooks
            ModHooks.HeroUpdateHook -= OnHeroUpdate;
            ModHooks.AfterPlayerDeadHook -= OnPlayerDeath;

            On.BossSceneController.CheckBossesDead -= BossSceneController_CheckBossesDead;
            //On.BossSceneController.EndSceneDelayed -= BossSceneController_EndSceneDelayed;

            Log("Unloaded");
#endif
        }

        /// 
        /// Events
        /// 

        private string OnSceneLoad(string sceneName)
        {
#if DEBUG
            Log($"OnSceneLoad: {sceneName}");
#endif

            messageBus.Put(new SceneChange { Name = sceneName });
            return sceneName;
        }

        private void OnEndBossScene(On.BossSceneController.orig_EndBossScene orig, BossSceneController self)
        {
#if DEBUG
            Log($"OnEndBossScene");
#endif

            // At least one boss died.
            // Note that this event can trigger twice in a fight (e.g. Oro and Mato).
            messageBus.Put(new BossDeath());

            orig(self);
        }

#if DEBUG
        private void OnHeroUpdate()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                string json = JsonConvert.SerializeObject(localData);
                Log("Current local data: " + json);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                ModDisplay.instance.Create();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                ModDisplay.instance.Destroy();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                ModDisplay.instance.Redraw();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                ModDisplay.instance.Show();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                ModDisplay.instance.Hide();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                ModDisplay.instance.Notify("Godhome Win Loss Tracker");
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                ModDisplay.instance.TextPosition = new Vector2(
                    ModDisplay.instance.TextPosition.x - 0.01f,
                    ModDisplay.instance.TextPosition.y
                );
                Log($"{ModDisplay.instance.TextPosition.x,0:F2} {ModDisplay.instance.TextPosition.y,0:F2}");
                ModDisplay.instance.Redraw();
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                ModDisplay.instance.TextPosition = new Vector2(
                    ModDisplay.instance.TextPosition.x + 0.01f,
                    ModDisplay.instance.TextPosition.y
                );
                Log($"{ModDisplay.instance.TextPosition.x,0:F2} {ModDisplay.instance.TextPosition.y,0:F2}");
                ModDisplay.instance.Redraw();
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                ModDisplay.instance.TextPosition = new Vector2(
                    ModDisplay.instance.TextPosition.x,
                    ModDisplay.instance.TextPosition.y + 0.01f
                );
                Log($"{ModDisplay.instance.TextPosition.x,0:F2} {ModDisplay.instance.TextPosition.y,0:F2}");
                ModDisplay.instance.Redraw();
            }
            else if (Input.GetKeyDown(KeyCode.G))
            {
                ModDisplay.instance.TextPosition = new Vector2(
                    ModDisplay.instance.TextPosition.x,
                    ModDisplay.instance.TextPosition.y - 0.01f
                );
                Log($"{ModDisplay.instance.TextPosition.x,0:F2} {ModDisplay.instance.TextPosition.y,0:F2}");
                ModDisplay.instance.Redraw();
            }
        }

        private void OnPlayerDeath()
        {
            messageBus.Put(new TKDeath());
        }
#endif

        ///
        /// IGlobalSettings<GlobalData>
        ///
        public void OnLoadGlobal(GlobalData data) => globalData = data;
        public GlobalData OnSaveGlobal() => globalData;

        /// 
        /// ILocalSettings<LocalData>
        ///
        public void OnLoadLocal(LocalData data) => localData = data;
        public LocalData OnSaveLocal() => localData;
    }
}
