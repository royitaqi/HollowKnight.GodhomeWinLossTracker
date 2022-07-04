using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Modding;
using UnityEngine;
using Newtonsoft.Json;
using Vasi;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker
{
    public class GodhomeWinLossTracker : Mod, IGodhomeWinLossTracker
    {
        internal static GodhomeWinLossTracker instance;
        public GlobalData globalData { get; set; } = new GlobalData();
        public LocalData localData { get; set; } = new LocalData();
        public FolderData folderData { get; set; } = new FolderData();

        ///
        /// Mod
        ///

        // <breaking change>.<non-breaking big feature/fix>.<non-breaking small feature/fix>.<patch>
        public override string GetVersion() => "9.9.9.9";
        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            instance = this;

            // Production hooks
            //On.HeroController.TakeDamage += HeroController_TakeDamage; // Turn this line on and off to see GodSeeker+'s Half Damage feature ineffective and effective, respectively
            On.PlayerData.TakeHealth += PlayerData_TakeHealth;
        }

        private void PlayerData_TakeHealth(On.PlayerData.orig_TakeHealth orig, PlayerData self, int amount)
        {
            int healthBefore = PlayerData.instance.health + PlayerData.instance.healthBlue;
            orig(self, amount);
            int healthAfter = PlayerData.instance.health + PlayerData.instance.healthBlue;
            int damage = healthBefore - healthAfter;

            if (damage != 0)
            {
                Log($"DEBUG Z: damage={damage} healthAfter={healthAfter}");
            }
        }

        private void HeroController_TakeDamage(On.HeroController.orig_TakeDamage orig, HeroController self, GameObject go, GlobalEnums.CollisionSide damageSide, int damageAmount, int hazardType)
        {
            orig(self, go, damageSide, damageAmount, hazardType);
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

        public void OnLoadLocal(LocalData data)
        {
            localData = data;
        }
        public LocalData OnSaveLocal()
        {
            return localData;
        }
    }
}
