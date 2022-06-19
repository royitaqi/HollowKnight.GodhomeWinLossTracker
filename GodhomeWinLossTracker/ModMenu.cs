﻿using System;
using System.Linq;
using Modding;
using Satchel.BetterMenus;

namespace GodhomeWinLossTracker
{
    public static class ModMenu
    {
        private static Menu _menuRef;

        private static Menu PrepareMenu(ModToggleDelegates toggle)
        {
            return new Menu("Godhome Win Loss Tracker", new Element[]
            {
                toggle.CreateToggle("Mod toggle", "Allows disabling the mod")
            });
        }

        public static MenuScreen GetMenu(MenuScreen lastMenu, ModToggleDelegates? toggle)
        {
            if (toggle == null) return null;
            if (_menuRef == null) {
                _menuRef = PrepareMenu((ModToggleDelegates)toggle);
            }
            return _menuRef.GetMenuScreen(lastMenu);
        }
    }
}
