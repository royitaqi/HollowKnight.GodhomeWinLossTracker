﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
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
#if (DEBUG)
        public override string GetVersion() => version.Value + "-DEBUG";
#else
        public override string GetVersion() => version.Value;
#endif
        private static readonly Lazy<string> version = new(() =>
        {
            Assembly asm = typeof(GodhomeWinLossTracker).Assembly;
            string ver = asm.GetName().Version.ToString();
            using var sha = SHA256.Create();
            using FileStream stream = File.OpenRead(asm.Location);
            byte[] hashBytes = sha.ComputeHash(stream);
            string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            return $"{ver}-{hash.Substring(0, 6)}";
        });

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
#if DEBUG
                new Debugger(),
#endif
                new DisplayInvoker(str => str.Localize(), ModDisplay.instance.Notify),
                new EnemyStateObserver(),
                new FightTracker(() => GameManagerUtils.PlayTimeMs),
                new HoGStatsQueryProcessor(str => str.Localize()),
                new PantheonStatsQueryProcessor(str => str.Localize()),
                new PhaseTracker(() => GameManagerUtils.PlayTimeMs),
                new RecordCollector(),
                new SaveLoad(),
                new SceneChangeObserver(),
                new ScreenCaptureTaker(),
                new SequenceChangeDetector(),
                new TKDeathAndStatusObserver(),
                new TKHpPosObserver()
            };
            messageBus = new(this, handlers);

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
