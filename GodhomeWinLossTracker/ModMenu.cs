using System;
using System.Linq;
using Modding;
using Satchel.BetterMenus;
using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker
{
    public static class ModMenu
    {
        private static Menu _menuRef;

        private static Menu PrepareMenu()
        {
            return new Menu("ModName".Localize(), new Element[]
            {
                new HorizontalOption(
                    "Menu/Show stats in challenge menus".Localize(),
                    "Menu/Both HoG and pantheons".Localize(),
                    new []{ "Menu/Off".Localize(), "Menu/On".Localize() },
                    selectedIndex => {
                        GodhomeWinLossTracker.instance.globalData.ShowStatsInChallengeMenu = selectedIndex == 1;
                    },
                    () => GodhomeWinLossTracker.instance.globalData.ShowStatsInChallengeMenu ? 1 : 0
                ),
                new HorizontalOption(
                    "Menu/Notify win/loss".Localize(),
                    "",
                    new []{ "Menu/Off".Localize(), "Menu/On".Localize() },
                    selectedIndex => {
                        GodhomeWinLossTracker.instance.globalData.NotifyForRecord = selectedIndex == 1;
                    },
                    () => GodhomeWinLossTracker.instance.globalData.NotifyForRecord ? 1 : 0
                ),
                new HorizontalOption(
                    "Menu/Notify personal best time".Localize(),
                    "Menu/In win notifications".Localize(),
                    new []{ "Menu/Off".Localize(), "Menu/On".Localize() },
                    selectedIndex => {
                        GodhomeWinLossTracker.instance.globalData.NotifyPBTime = selectedIndex == 1;
                    },
                    () => GodhomeWinLossTracker.instance.globalData.NotifyPBTime ? 1 : 0
                ),
                new HorizontalOption(
                    "Menu/Notify exports".Localize(),
                    "",
                    new []{ "Menu/Off".Localize(), "Menu/On".Localize() },
                    selectedIndex => {
                        GodhomeWinLossTracker.instance.globalData.NotifyForExport = selectedIndex == 1;
                    },
                    () => GodhomeWinLossTracker.instance.globalData.NotifyForExport ? 1 : 0
                ),
                new HorizontalOption(
                    "Menu/Auto export stats".Localize(),
                    "Menu/When saving games".Localize(),
                    new []{ "Menu/Off".Localize(), "Menu/On".Localize() },
                    selectedIndex => {
                        GodhomeWinLossTracker.instance.globalData.AutoExport = selectedIndex == 1;
                    },
                    () => GodhomeWinLossTracker.instance.globalData.AutoExport ? 1 : 0
                ),
                new MenuButton(
                    "Menu/Export stats now".Localize(),
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
