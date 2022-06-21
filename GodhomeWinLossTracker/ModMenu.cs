using System;
using System.Linq;
using Modding;
using Satchel.BetterMenus;
using GodhomeWinLossTracker.MessageBus.Messages;

namespace GodhomeWinLossTracker
{
    public static class ModMenu
    {
        private static Menu _menuRef;

        private static Menu PrepareMenu(ModToggleDelegates toggle)
        {
            return new Menu("Godhome Win Loss Tracker", new Element[]
            {
                toggle.CreateToggle("Mod toggle", "Allows disabling the mod"),
                new MenuButton(
                    "Export stats as TSV",
                    "",
                    _ => ExportStatsAsTsv()
                ),
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

        public static void ExportStatsAsTsv()
        {
            GodhomeWinLossTracker.instance.Log("Exporting stats as TSV");

            GodhomeWinLossTracker.instance.messageBus.Put(new ExportFolderData());

            GodhomeWinLossTracker.instance.Log("Exported stats as TSV");
        }
    }
}
