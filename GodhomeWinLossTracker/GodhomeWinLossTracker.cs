using System;
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
        public override string GetVersion() => "0.2.0.1";
        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
#if DEBUG
            Log("Initializing mod");
#endif
            instance = this;

            IHandler[] handlers = new IHandler[] {
#if DEBUG
                // Put logger first, so that it prints messages on the bus before other handlers can handle it.
                new MessageBus.Handlers.Logger(),
#endif
                new BossChangeDetector(),
                new DisplayUpdater(this),
                new HoGStatsQueryProcessor(this),
                new PantheonStatsQueryProcessor(this),
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
            On.BossDoorChallengeUI.Setup += BossDoorChallengeUI_Setup;
            On.BossChallengeUI.Setup += BossChallengeUI_Setup;
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
            On.BossDoorChallengeUI.Setup -= BossDoorChallengeUI_Setup;
            On.BossChallengeUI.Setup -= BossChallengeUI_Setup;
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
            Log("OnEndBossScene");
#endif
            // At least one boss died.
            // Note that this event can trigger twice in a fight (e.g. Oro and Mato).
            messageBus.Put(new BossDeath());

            orig(self);
        }

        private void BossDoorChallengeUI_Setup(On.BossDoorChallengeUI.orig_Setup orig, BossDoorChallengeUI self, BossSequenceDoor door)
        {
            orig(self, door);

            if (globalData.ShowStatsInChallengeMenu)
            {
                messageBus.Put(new PantheonStatsQuery(self.titleTextMain.text, (runs, pb, churns) =>
                {
                    if (runs != null)
                    {
                        self.titleTextSuper.text = runs;
                    }
                    if (pb != null)
                    {
                        self.titleTextMain.text = pb;
                    }
                    if (churns != null)
                    {
                        self.descriptionText.text = churns;
                    }
                }));
            }
        }

        private void BossChallengeUI_Setup(On.BossChallengeUI.orig_Setup orig, BossChallengeUI self, BossStatue bossStatue, string bossNameSheet, string bossNameKey, string descriptionSheet, string descriptionKey)
        {
            orig(self, bossStatue, bossNameSheet, bossNameKey, descriptionSheet, descriptionKey);

            if (globalData.ShowStatsInChallengeMenu)
            {
                messageBus.Put(new HoGStatsQuery(self.bossNameText.text, statsText =>
                {
                    if (statsText != null)
                    {
                        self.descriptionText.text = statsText;
                    }
                }));
            }
        }

#if DEBUG
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
