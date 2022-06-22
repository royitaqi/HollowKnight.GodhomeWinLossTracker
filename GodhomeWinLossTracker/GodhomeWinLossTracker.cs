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
        internal TheMessageBus messageBus;
        public GlobalData globalData = new GlobalData();
        internal LocalData localData = new LocalData();
        internal FolderData folderData = new FolderData();

        ///
        /// Mod
        ///

        public override string GetVersion() => "0.1.0.0";
        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
#if DEBUG
            Log("Initializing mod");
#endif
            instance = this;

            messageBus = new(this);

            // Production hooks
            ModHooks.BeforeSceneLoadHook += OnSceneLoad;
            On.BossSceneController.EndBossScene += OnEndBossScene;
#if DEBUG
            // Debug hooks
            ModHooks.HeroUpdateHook += OnHeroUpdate;
#endif
            ModDisplay.Initialize();
#if DEBUG
            Log("Initialized");
#endif
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
                string json = JsonConvert.SerializeObject(folderData);
                Log("Current local data: " + json);

                messageBus.Put(new SaveFolderData());
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
#endif

        ///
        /// IGlobalSettings<GlobalData>
        ///
        public void OnLoadGlobal(GlobalData data) => globalData = data;
        public GlobalData OnSaveGlobal() => globalData;

        /// 
        /// ILocalSettings<LocalData>
        ///
        public void OnLoadLocal(LocalData data)
        {
            Log($"DEBUG PlayerData.instance.profileID = {PlayerData.instance.profileID}");
            Log($"DEBUG GameManager.instance.profileID = {GameManager.instance.profileID}");
            localData = data;
#if DEBUG
            Log($"Loading local data (slot {localData.ProfileID})");
#endif
            // actual read
            if (messageBus != null)
            {
                messageBus.Put(new LoadFolderData());
            }
#if DEBUG
            Log($"Loaded local data (slot {localData.ProfileID})");
#endif
        }
        public LocalData OnSaveLocal()
        {
            Log($"DEBUG PlayerData.instance.profileID = {PlayerData.instance.profileID}");
            Log($"DEBUG GameManager.instance.profileID = {GameManager.instance.profileID}");
            localData.ProfileID = GameManager.instance.profileID;
#if DEBUG
            Log($"Saving local data (slot {localData.ProfileID})");
#endif
            // actual save
            if (messageBus != null)
            {
                messageBus.Put(new SaveFolderData());
            }
#if DEBUG
            Log($"Saved local data (slot {localData.ProfileID})");
#endif
            return localData;
        }
    }
}
