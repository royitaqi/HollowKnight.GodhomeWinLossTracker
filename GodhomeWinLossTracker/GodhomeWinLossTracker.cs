using System.Collections.Generic;
using GodhomeWinLossTracker.MessageBus;
using GodhomeWinLossTracker.MessageBus.Handlers;
using GodhomeWinLossTracker.Utils;
using Modding;
using UnityEngine;
using Logger = GodhomeWinLossTracker.MessageBus.Handlers.Logger;

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
        public override string GetVersion() => "0.4.11.3";
        // Make sure this mod is loaded after GodSeeker+.
        public override int LoadPriority() => 5;
        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            this.LogMod("Initializing mod");

            instance = this;

            // Initialize ModDisplay so that its instance can be used to initialize DisplayInvoker below.
            ModDisplay.Initialize();

            Handler[] handlers = new Handler[] {
                // Put logger first, so that it prints messages on the bus before other handlers can handle it.
                new Logger(),
                new BossChangeDetector(),
                new BossDeathObserver(),
                new BossHpPosUpdater(),
                new ChallengeMenuInjector(),
                new Debugger(),
                new DisplayInvoker(str => str.Localize(), ModDisplay.instance.Notify),
                new EnemyStateObserver(),
                new FightTracker(() => GameManagerUtils.PlayTimeMs),
                new GameLoadDetector(),
                new HoGStatsQueryProcessor(str => str.Localize()),
                new PantheonStatsQueryProcessor(str => str.Localize()),
                new RecordCollector(),
                new SaveLoad(),
                new SceneChangeObserver(),
                new SequenceChangeDetector(),
                new TKDeathDetector(),
                new TKHealthObserver()
            };
            messageBus = new(this, handlers);

#if DEBUG
            // Turn this line on/off to get FSM related events
            //FsmUtils.Load(this, fsm => fsm.gameObject.name == "Mage Knight" && fsm.FsmName == "Mage Knight");
            //FsmUtils.Load(this, fsm => fsm.gameObject.name == "Giant Fly" && fsm.FsmName == "Big Fly Control");
            //FsmUtils.Load(this);

            // Turn this line on/off to get ModDisplay related backdoors
            //ModDisplayUtils.Initialize(); 
#endif

            this.LogMod("Initialized");
        }

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

        public void OnLoadLocal(LocalData data) => localData = data;
        public LocalData OnSaveLocal() => localData;
    }
}
