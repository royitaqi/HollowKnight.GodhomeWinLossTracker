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
using GodhomeWinLossTracker.Utils;

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
        public override string GetVersion() => "0.3.0.0";
        
        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
#if DEBUG
            Log("Initializing mod");
#endif
            instance = this;

            Handler[] handlers = new Handler[] {
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
            On.PlayerData.TakeHealth += PlayerData_TakeHealth;
            On.PlayerData.AddHealth += PlayerData_AddHealth;
#if DEBUG
            // Debug hooks
            ModHooks.HeroUpdateHook += OnHeroUpdate;
            On.HeroController.Start += HeroController_Start;
            On.GameManager.Start += GameManager_Start;
            //FsmUtils.Initialize(); // Turn this line on/off to get FSM related events
            //ModDisplayUtils.Initialize(); // Turn this line on/off to get ModDisplay related backdoors
#endif

            ModDisplay.Initialize();

#if DEBUG
            Log("Initialized");
#endif
        }

        private void PlayerData_AddHealth(On.PlayerData.orig_AddHealth orig, PlayerData self, int amount)
        {
            int healthBefore = PlayerData.instance.health + PlayerData.instance.healthBlue;
            orig(self, amount);
            int healthAfter = PlayerData.instance.health + PlayerData.instance.healthBlue;
            int heal = healthAfter - healthBefore;

            if (heal != 0)
            {
                messageBus.Put(new TKHeal { Heal = heal, HealthAfter = healthAfter });
            }
        }

        private void PlayerData_TakeHealth(On.PlayerData.orig_TakeHealth orig, PlayerData self, int amount)
        {
            int healthBefore = PlayerData.instance.health + PlayerData.instance.healthBlue;
            orig(self, amount);
            int healthAfter = PlayerData.instance.health + PlayerData.instance.healthBlue;
            int damage = healthBefore - healthAfter;

            if (damage != 0)
            {
                messageBus.Put(new TKHit { Damage = damage, HealthAfter = healthAfter });
            }
        }

        private void GameManager_Start(On.GameManager.orig_Start orig, GameManager self)
        {
            Log($"DEBUG GameManager_Start");
            orig(self);
        }

        private void HeroController_Start(On.HeroController.orig_Start orig, HeroController self)
        {
            Log($"DEBUG HeroController_Start");
            orig(self);
        }

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
                Log(DevUtils.DumpLogCount());
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Log($"DEBUG >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

                Log($"DEBUG PlayTime={GameManager.instance.PlayTime,0:F3}");

                float sessionStartTime = ReflectionHelper.GetField<GameManager, float>(GameManager.instance, "sessionStartTime");
                Log($"DEBUG sessionStartTime={sessionStartTime,0:F3}");

                float sessionPlayTimer = ReflectionHelper.GetField<GameManager, float>(GameManager.instance, "sessionPlayTimer");
                Log($"DEBUG sessionPlayTimer={sessionPlayTimer,0:F3}");

                float sumFloat = sessionStartTime + sessionPlayTimer;
                Log($"DEBUG float sum={sumFloat,0:F3}");

                double sumDouble = (double)sessionStartTime + (double)sessionPlayTimer;
                Log($"DEBUG double sum={sumDouble,0:F3}");

                float sumDoubleFloat = (float)sumDouble;
                Log($"DEBUG double-float sum={sumDoubleFloat,0:F3}");

                Log($"DEBUG -----------------------------------------------");

                Log($"DEBUG PlayTimeMs={GameManagerUtils.PlayTimeMs}");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
            }
        }
#endif

        ///
        /// ICustomMenuMod
        ///

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggle) => ModMenu.GetMenu(modListMenu, toggle);

        public bool ToggleButtonInsideMenu => false;

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
