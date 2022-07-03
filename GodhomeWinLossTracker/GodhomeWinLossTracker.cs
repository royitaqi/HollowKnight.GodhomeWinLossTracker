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
        public override string GetVersion() => "0.2.4.0";
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
                new GameLoadDetector(),
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
            On.PlayMakerFSM.Start += PlayMakerFSM_Start;
            On.PlayMakerFSM.SetState += PlayMakerFSM_SetState;
            On.PlayMakerFSM.Update += PlayMakerFSM_Update;
            On.PlayMakerFSM.SendEvent += PlayMakerFSM_SendEvent;
            On.PlayMakerFSM.SendRemoteFsmEvent += PlayMakerFSM_SendRemoteFsmEvent;
            On.PlayMakerFSM.ChangeState_FsmEvent += PlayMakerFSM_ChangeState_FsmEvent;
            On.HeroController.Start += HeroController_Start;
            On.GameManager.Start += GameManager_Start;
#endif
            ModDisplay.Initialize();
#if DEBUG
            Log("Initialized");
#endif
        }

        private void GameManager_Start(On.GameManager.orig_Start orig, GameManager self)
        {
            DevUtils.Log($"DEBUG GameManager_Start");
            orig(self);
        }

        private void HeroController_Start(On.HeroController.orig_Start orig, HeroController self)
        {
            DevUtils.Log($"DEBUG HeroController_Start");
            orig(self);
        }

        private void PlayMakerFSM_Start(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
        {
            DevUtils.Log($"DEBUG PlayMakerFSM_Start: GO={self.gameObject.name} FsmName={self.FsmName} ActiveStateName={self.ActiveStateName}");
            orig(self);
        }
        private void PlayMakerFSM_SetState(On.PlayMakerFSM.orig_SetState orig, PlayMakerFSM self, string stateName)
        {
            DevUtils.Log($"DEBUG PlayMakerFSM_SetState: GO={self.gameObject.name} FsmName={self.FsmName} stateName={stateName}");
            orig(self, stateName);
        }

        private void PlayMakerFSM_Update(On.PlayMakerFSM.orig_Update orig, PlayMakerFSM self)
        {
            orig(self);
            DevUtils.Log($"DEBUG PlayMakerFSM_Update: GO={self.gameObject.name} FsmName={self.FsmName} ActiveStateName={self.ActiveStateName}");
        }

        private void PlayMakerFSM_SendEvent(On.PlayMakerFSM.orig_SendEvent orig, PlayMakerFSM self, string eventName)
        {
            DevUtils.Log($"DEBUG PlayMakerFSM_SendEvent: GO={self.gameObject.name} FsmName={self.FsmName} eventName={eventName}");

            if (self.FsmName == "Knight Damage")
            {
                Log($"TK health = {PlayerData.instance.health + PlayerData.instance.healthBlue}");
            }

            orig(self, eventName);
        }

        private void PlayMakerFSM_SendRemoteFsmEvent(On.PlayMakerFSM.orig_SendRemoteFsmEvent orig, PlayMakerFSM self, string eventName)
        {
            DevUtils.Log($"DEBUG PlayMakerFSM_SendRemoteFsmEvent: GO={self.gameObject.name} FsmName={self.FsmName} eventName={eventName}");
            orig(self, eventName);
        }

        private void PlayMakerFSM_ChangeState_FsmEvent(On.PlayMakerFSM.orig_ChangeState_FsmEvent orig, PlayMakerFSM self, HutongGames.PlayMaker.FsmEvent fsmEvent)
        {
            DevUtils.Log($"DEBUG PlayMakerFSM_ChangeState_FsmEvent: GO={self.gameObject.name} FsmName={self.FsmName} eventName={fsmEvent.Name}");
            orig(self, fsmEvent);
        }


        ///
        /// ICustomMenuMod
        ///

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggle) => ModMenu.GetMenu(modListMenu, toggle);

        public bool ToggleButtonInsideMenu => false;

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
                int? indexq = GodhomeUtils.GetPantheonIndexFromDescriptionKey(door.descriptionKey);
                if (indexq == null)
                {
                    // Unknown pantheon. No change to challenge menu.
                    return;
                }
                int index = (int)indexq;

                messageBus.Put(new PantheonStatsQuery(index, (runs, pb, churns) =>
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
                messageBus.Put(new HoGStatsQuery(bossNameKey, statsText =>
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
                Log(DevUtils.DumpLog());
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
