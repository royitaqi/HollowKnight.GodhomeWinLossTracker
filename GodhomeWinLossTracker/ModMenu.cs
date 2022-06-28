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

        private static Menu PrepareMenu()
        {
            return new Menu("Godhome Win Loss Tracker", new Element[]
            {
                new HorizontalOption(
                    "Show stats in challenge menus",
                    "Both HoG and pantheons",
                    new []{ "Off", "On" },
                    selectedIndex => {
                        GodhomeWinLossTracker.instance.globalData.ShowStatsInChallengeMenu = selectedIndex == 1;
                    },
                    () => GodhomeWinLossTracker.instance.globalData.ShowStatsInChallengeMenu ? 1 : 0
                ),
                new HorizontalOption(
                    "Notify win/loss",
                    "",
                    new []{ "Off", "On" },
                    selectedIndex => {
                        GodhomeWinLossTracker.instance.globalData.NotifyForRecord = selectedIndex == 1;
                    },
                    () => GodhomeWinLossTracker.instance.globalData.NotifyForRecord ? 1 : 0
                ),
                new HorizontalOption(
                    "Notify personal best time",
                    "In wins",
                    new []{ "Off", "On" },
                    selectedIndex => {
                        GodhomeWinLossTracker.instance.globalData.NotifyPBTime = selectedIndex == 1;
                    },
                    () => GodhomeWinLossTracker.instance.globalData.NotifyPBTime ? 1 : 0
                ),
                new HorizontalOption(
                    "Notify exports",
                    "",
                    new []{ "Off", "On" },
                    selectedIndex => {
                        GodhomeWinLossTracker.instance.globalData.NotifyForExport = selectedIndex == 1;
                    },
                    () => GodhomeWinLossTracker.instance.globalData.NotifyForExport ? 1 : 0
                ),
                new HorizontalOption(
                    "Auto export stats",
                    "When saving games",
                    new []{ "Off", "On" },
                    selectedIndex => {
                        GodhomeWinLossTracker.instance.globalData.AutoExport = selectedIndex == 1;
                    },
                    () => GodhomeWinLossTracker.instance.globalData.AutoExport ? 1 : 0
                ),
                new MenuButton(
                    "Export stats now",
                    "",
                    _ => ExportStatsAsTsv()
                ),
            });
        }

        public static MenuScreen GetMenu(MenuScreen lastMenu, ModToggleDelegates? toggle)
        {
            DevUtils.Assert(toggle == null, "This mod is non-toggleable");
            if (_menuRef == null) {
                _menuRef = PrepareMenu();
            }
            return _menuRef.GetMenuScreen(lastMenu);
        }

        public static void ExportStatsAsTsv()
        {
            GodhomeWinLossTracker.instance.messageBus.Put(new ExportFolderData());
        }
    }
}
