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

        // <breaking change>.<non-breaking major feature/fix>.<non-breaking minor feature/fix>.<patch>
        public override string GetVersion() => "0.4.3.0";
        
        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            this.LogMod("Initializing mod");

            instance = this;

            Handler[] handlers = new Handler[] {
                // Put logger first, so that it prints messages on the bus before other handlers can handle it.
                new MessageBus.Handlers.Logger(),
                new BossChangeDetector(),
                new BossDeathObserver(),
                new BossHPUpdater(),
                new ChallengeMenuInjector(),
                new DisplayInvoker(this),
                new EnemyStateObserver(),
                new FightTracker(() => GameManagerUtils.PlayTimeMs),
                new GameLoadDetector(),
                new HoGStatsQueryProcessor(this, str => str.Localize()),
                new PantheonStatsQueryProcessor(this, str => str.Localize()),
                new RecordCollector(this),
                new SaveLoad(this),
                new SceneChangeObserver(),
                new SequenceChangeDetector(),
                new TKDeathDetector(),
                new TKHealthObserver()
            };
            messageBus = new(this, handlers);

#if DEBUG
            // Debug hooks
            ModHooks.HeroUpdateHook += OnHeroUpdate;
            On.HeroController.Start += HeroController_Start;
            On.GameManager.Start += GameManager_Start;

            // Turn this line on/off to get FSM related events
            //FsmUtils.Load(this, fsm => fsm.gameObject.name == "Mage Knight" && fsm.FsmName == "Mage Knight");
            //FsmUtils.Load(this, fsm => fsm.gameObject.name == "Giant Fly" && fsm.FsmName == "Big Fly Control");
            //FsmUtils.Load(this);

            // Turn this line on/off to get ModDisplay related backdoors
            //ModDisplayUtils.Initialize(); 
#endif

            ModDisplay.Initialize();

            this.LogMod("Initialized");
        }

#if DEBUG
        private void GameManager_Start(On.GameManager.orig_Start orig, GameManager self)
        {
            this.LogMod($"GameManager_Start");
            orig(self);
        }

        private void HeroController_Start(On.HeroController.orig_Start orig, HeroController self)
        {
            this.LogMod($"HeroController_Start");
            orig(self);
        }

        private void OnHeroUpdate()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                this.LogMod(JsonConvert.SerializeObject(folderData));
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                this.LogMod(LoggingUtils.DumpLogCount());
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
            this.LogMod($"Loading local data (slot {localData.ProfileID})");
            // actual read
            if (messageBus != null)
            {
                messageBus.Put(new LoadFolderData());
            }
            this.LogMod($"Loaded local data (slot {localData.ProfileID})");
        }
        public LocalData OnSaveLocal()
        {
            localData.ProfileID = GameManager.instance.profileID;
            this.LogMod($"Saving local data (slot {localData.ProfileID})");
            // actual save
            if (messageBus != null)
            {
                messageBus.Put(new SaveFolderData());
            }
            this.LogMod($"Saved local data (slot {localData.ProfileID})");
            return localData;
        }
    }
}
