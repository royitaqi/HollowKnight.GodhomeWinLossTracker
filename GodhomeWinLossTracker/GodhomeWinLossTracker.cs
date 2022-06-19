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

        public override string GetVersion() => "v0.0.0.3";
        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing mod");

            instance = this;

            messageBus = new();
            messageBus.Initialize(this);

            ModHooks.BeforeSceneLoadHook += OnSceneLoad;
            ModHooks.HeroUpdateHook += OnHeroUpdate;
            On.BossSceneController.EndBossScene += OnEndBossScene;

            Log("Initialized mod");
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
            Log("Unloading");

            ModHooks.BeforeSceneLoadHook -= OnSceneLoad;
            ModHooks.HeroUpdateHook -= OnHeroUpdate;
            On.BossSceneController.EndBossScene -= OnEndBossScene;

            Log("Unloaded");
        }

        /// 
        /// Events
        /// 

        private string OnSceneLoad(string sceneName)
        {
            messageBus.Put(new SceneChange { Name = sceneName });
            return sceneName;
        }

        private void OnHeroUpdate()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                string json = JsonConvert.SerializeObject(localData);
                Log("Current local data: " + json);
            }
        }

        private void OnEndBossScene(On.BossSceneController.orig_EndBossScene orig, BossSceneController self)
        {
            // At least one boss died.
            // Note that this event can trigger twice in a fight (e.g. Oro and Mato).
            messageBus.Put(new BossDeath());

            orig(self);
        }

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
