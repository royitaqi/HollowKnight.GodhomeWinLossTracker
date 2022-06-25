﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Modding;
using UnityEngine;
using GodhomeWinLossTracker.MessageBus;
using GodhomeWinLossTracker.MessageBus.Handlers;
using GodhomeWinLossTracker.MessageBus.Messages;
using Newtonsoft.Json;
using Vasi;

namespace GodhomeWinLossTracker
{
    public class GodhomeWinLossTracker : Mod, IGodhomeWinLossTracker
    {
        internal static GodhomeWinLossTracker instance;
        internal TheMessageBus messageBus;
        public GlobalData globalData { get; set; } = new GlobalData();
        public LocalData localData { get; set; } = new LocalData();
        public FolderData folderData { get; set; } = new FolderData();

        ///
        /// Mod
        ///

        // <breaking change>.<non-breaking big feature/fix>.<non-breaking small feature/fix>.<patch>
        public override string GetVersion() => "0.1.4.0";
        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
#if DEBUG
            Log("Initializing mod");
#endif
            instance = this;

            IHandler[] handlers = new IHandler[] {
#if DEBUG
                new MessageBus.Handlers.Logger(),
#endif
                new BossChangeDetector(),
                new DisplayUpdater(this),
                new SaveLoad(this),
                new SequenceChangeDetector(),
                new TKDeathDetector(),
                new WinLossGenerator(),
                new WinLossTracker(this)
            };
            messageBus = new(this, handlers);

            // Production hooks
            ModHooks.BeforeSceneLoadHook += OnSceneLoad;
            On.BossSceneController.EndBossScene += OnEndBossScene;
#if DEBUG
            // Debug hooks
            ModHooks.HeroUpdateHook += OnHeroUpdate;
            On.BossDoorChallengeUI.ShowSequence += ApplyBindingStates;
            On.BossDoorChallengeUI.HideSequence += RecordBindingStates;
            On.BossChallengeUI.Setup += BossChallengeUI_Setup;
            On.BossChallengeUI.Start += BossChallengeUI_Start;
            On.BossChallengeUI.Hide += BossChallengeUI_Hide;
#endif
            ModDisplay.Initialize();
#if DEBUG
            Log("Initialized");
#endif
        }

        private void BossChallengeUI_Hide(On.BossChallengeUI.orig_Hide orig, BossChallengeUI self)
        {
            Log("DEBUG BossChallengeUI_Hide");
            orig(self);
        }

        private void BossChallengeUI_Start(On.BossChallengeUI.orig_Start orig, BossChallengeUI self)
        {
            Log("DEBUG BossChallengeUI_Start");
            orig(self);
        }

        private void BossChallengeUI_Setup(On.BossChallengeUI.orig_Setup orig, BossChallengeUI self, BossStatue bossStatue, string bossNameSheet, string bossNameKey, string descriptionSheet, string descriptionKey)
        {
            Log($"DEBUG BossChallengeUI_Setup");
            Log($"DEBUG bossStatue.bossScene.Tier1Scene = {bossStatue.bossScene.Tier1Scene}");
            Log($"DEBUG bossStatue.bossScene.Tier2Scene = {bossStatue.bossScene.Tier2Scene}");
            Log($"DEBUG bossStatue.bossScene.Tier3Scene = {bossStatue.bossScene.Tier3Scene}");
            Log($"DEBUG bossNameSheet = {bossNameSheet}");
            Log($"DEBUG bossNameKey = {bossNameKey}");
            Log($"DEBUG descriptionSheet = {descriptionSheet}");
            Log($"DEBUG descriptionKey = {descriptionKey}");
            orig(self, bossStatue, bossNameSheet, bossNameKey, descriptionSheet, descriptionKey);
        }

        ///
        /// ICustomMenuMod
        ///

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggle) => ModMenu.GetMenu(modListMenu, toggle);

        ///
        /// ITogglableMod
        ///

        public bool ToggleButtonInsideMenu => true;

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
            On.BossDoorChallengeUI.ShowSequence -= ApplyBindingStates;
            On.BossDoorChallengeUI.HideSequence -= RecordBindingStates;

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
            Log("OnEndBossScene");
#endif
            // At least one boss died.
            // Note that this event can trigger twice in a fight (e.g. Oro and Mato).
            messageBus.Put(new BossDeath());

            orig(self);
        }

#if DEBUG
        private IEnumerator ApplyBindingStates(On.BossDoorChallengeUI.orig_ShowSequence orig, BossDoorChallengeUI self)
        {
            yield return orig(self);
            Log("DEBUG ApplyBindingStates");
        }

        private IEnumerator RecordBindingStates(On.BossDoorChallengeUI.orig_HideSequence orig, BossDoorChallengeUI self, bool sendEvent)
        {
            Log("DEBUG RecordBindingStates");
            yield return orig(self, sendEvent);
        }

        private void OnHeroUpdate()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                string json = JsonConvert.SerializeObject(folderData);
                Log("Current local data: " + json);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Log("DEBUG hooking Anim Start");
                GameObject.Find("Knight").transform.Find("Hero Death").gameObject.LocateMyFSM("Hero Death Anim").GetState("Anim Start").AddMethod(() =>
                {
                    Log("DEBUG Anim Start");
                    PlayerData.instance.SetInt("health", 2);
                    PlayerData.instance.SetInt("geo", 20);
                });
                Log("DEBUG hooked Anim Start");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Log("DEBUG hooking Map Zone");
                GameObject.Find("Knight").transform.Find("Hero Death").gameObject.LocateMyFSM("Hero Death Anim").GetState("Map Zone").AddMethod(() =>
                {
                    Log("DEBUG Map Zone");
                    PlayerData.instance.SetInt("health", 3);
                    PlayerData.instance.SetInt("geo", 30);
                });
                Log("DEBUG hooked Map Zone");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Log("DEBUG hooking UpSlash");
                GameObject.Find("Knight").transform.Find("UpSlash").gameObject.LocateMyFSM("damages_enemy").GetState("Send Event").AddMethod(() =>
                {
                    Log("DEBUG UpSlash");
                    PlayerData.instance.SetInt("health", 4);
                    PlayerData.instance.SetInt("geo", 40);
                });
                Log("DEBUG hooked UpSlash");
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
