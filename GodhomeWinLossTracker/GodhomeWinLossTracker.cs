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
        public override string GetVersion() => "0.4.1.0";
        
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
                new BossDeathObserver(),
                new BossHPObserver(),
                new ChallengeMenuInjector(),
                new DisplayUpdater(this),
                new EnemyStateObserver(),
                new GameLoadDetector(),
                new HoGStatsQueryProcessor(this, str => str.Localize()),
                new PantheonStatsQueryProcessor(this, str => str.Localize()),
                new SaveLoad(this),
                new SceneChangeObserver(),
                new SequenceChangeDetector(),
                new TKDeathDetector(),
                new TKHealthObserver(),
                new WinLossGenerator(() => GameManagerUtils.PlayTimeMs),
                new WinLossTracker(this)
            };
            messageBus = new(this, handlers);

#if DEBUG
            // Debug hooks
            ModHooks.HeroUpdateHook += OnHeroUpdate;
            On.HeroController.Start += HeroController_Start;
            On.GameManager.Start += GameManager_Start;

            // Turn this line on/off to get FSM related events
            FsmUtils.Load(fsm => fsm.gameObject.name == "Mage Knight" && fsm.name == "Mage Knight");

            // Turn this line on/off to get ModDisplay related backdoors
            //ModDisplayUtils.Initialize(); 
#endif

            ModDisplay.Initialize();

#if DEBUG
            Log("Initialized");
#endif
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

#if DEBUG
        private void OnHeroUpdate()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                Log(JsonConvert.SerializeObject(folderData));
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                Log(DevUtils.DumpLogCount());
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                messageBus.Put(new BusEvent { ForTest = true });
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
