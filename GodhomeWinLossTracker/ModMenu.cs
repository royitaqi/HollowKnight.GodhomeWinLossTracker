using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;
using Modding;
using Osmi.Utils;
using Satchel.BetterMenus;
using UnityEngine;
using UnityEngine.UI;
using MenuButton = Satchel.BetterMenus.MenuButton;

namespace GodhomeWinLossTracker
{
    public static class ModMenu
    {
        public static MenuScreen GetMenu(MenuScreen modListMenu, ModToggleDelegates? toggle)
        {
            DevUtils.Assert(toggle == null, "This mod is non-toggleable");

            // Create the mod.
            // Note that we don't cache this menu instance, because the localization setting may have been updated between two menu invocations.
            var menu = PrepareMenu();
            var menuScreen = menu.GetMenuScreen(modListMenu);

            // Localize Mods menu.
            // At this point, the mod's name should have been added to the Mods menu.
            LocalizeModsMenu(modListMenu);

            return menuScreen;
        }

        private static void LocalizeModsMenu(MenuScreen modListMenu)
        {
            GameObject btn = UIManager
                .instance
                .UICanvas
                .gameObject
                .Child(
                    "ModListMenu",
                    "Content",
                    "ScrollMask",
                    "ScrollingPane",
                    $"{nameof(GodhomeWinLossTracker)}_Settings"
                )!;
            if (btn != null)
            {
                btn.Child("Label")!.GetComponent<Text>().text =
                    "ModName".Localize() + ' ' + "Settings".Localize();
            }
        }

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
                    "Menu/Notify personal bests".Localize(),
                    "Menu/In win notifications".Localize(),
                    new []{ "Menu/Off".Localize(), "Menu/On".Localize() },
                    selectedIndex => {
                        GodhomeWinLossTracker.instance.globalData.NotifyPB = selectedIndex == 1;
                    },
                    () => GodhomeWinLossTracker.instance.globalData.NotifyPB ? 1 : 0
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
                new HorizontalOption(
                    "Menu/Async save".Localize(),
                    "Menu/Async save description".Localize(),
                    new []{ "Menu/Off".Localize(), "Menu/On".Localize() },
                    selectedIndex => {
                        GodhomeWinLossTracker.instance.globalData.AsyncWrites = selectedIndex == 1;
                    },
                    () => GodhomeWinLossTracker.instance.globalData.AsyncWrites? 1 : 0
                ),
#if DEBUG
                new MenuButton(
                    "DEBUG: Load FsmUtils",
                    "",
                    _ => FsmUtils.Load(GodhomeWinLossTracker.instance)
                ),
#endif
            });
        }


        private static void ExportStatsAsTsv()
        {
            GodhomeWinLossTracker.instance.messageBus.Put(new ExportFolderData { Slot = GameManager.instance.profileID });
        }
    }
}
