using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GodhomeWinLossTracker
{
    internal static class GodhomeUtils
    {
        internal static bool IsBossScene(string sceneName)
        {
            return !NonBossScenes.Contains(sceneName);
        }

        internal static bool IsNonBossScene(string sceneName)
        {
            return NonBossScenes.Contains(sceneName);
        }

        internal static string GetNullableBossName(string sceneName)
        {
            // Return null for non-boss scene
            if (IsNonBossScene(sceneName))
            {
                return null;
            }

            Debug.Assert(BossSceneToName.ContainsKey(sceneName), $"Boss scene name {sceneName} should exist in BossSceneToName");
            return BossSceneToName[sceneName];
        }

        private static int? GetPantheonIndex(string previousSceneName, string bossSceneName)
        {
            Debug.Assert(IsBossScene(bossSceneName), "bossSceneName should be passed in valid, i.e. a boss scene");

            // p1-4
            if (previousSceneName == "GG_Boss_Door_Entrance")
            {
                for (int p = 0; p < 4; p++)
                {
                    if (Array.IndexOf(PantheonBossSceneNames[p], bossSceneName) != -1)
                    {
                        return p;
                    }
                }
            }
            // p5
            // Note: One would normally also gate by the condition `previousSceneName == "GG_Atrium_Roof"`.
            // However, there is an unexpected extra load of the scene GG_Atrium after GG_Atrium_Roof, which messes up with the above condition.
            // See https://github.com/Clazex/HollowKnight.GodSeekerPlus/issues/30
            // For now, the workaround is to condition on either scenes after p1-4 have been looked at.
            else if ((previousSceneName == "GG_Atrium_Roof" || previousSceneName == "GG_Atrium") &&
                Array.IndexOf(PantheonBossSceneNames[4], bossSceneName) != -1)
            {
                return 4;
            }

            return null;
        }

        internal static string GetSequenceName(string previousSceneName, string bossSceneName)
        {
            Debug.Assert(IsBossScene(bossSceneName), "bossSceneName should be passed in valid, i.e. a boss scene");

            if (previousSceneName == "GG_Workshop")
            {
                return "HoG";
            }

            int? pantheonIndex = GetPantheonIndex(previousSceneName, bossSceneName);
            if (pantheonIndex != null)
            {
                return $"P{pantheonIndex + 1}";
            }

            return null;
        }

        // This is tracked by boss scene name.
        // The same boss name in different difficulties can require different number of kills. E.g. Vengefly Kings
        internal static int GetKillsRequiredToWin(string bossSceneName)
        {
            Debug.Assert(IsBossScene(bossSceneName), "bossSceneName should be passed in valid, i.e. a boss scene");
            if (BossSceneToKillsRequiredToWin.ContainsKey(bossSceneName))
            {
                return BossSceneToKillsRequiredToWin[bossSceneName];
            }
            else
            {
                return 1;
            }
        }



        private static readonly HashSet<string> NonBossScenes = new HashSet<string>
        {
            "End_Game_Completion",
            "GG_Atrium",
            "GG_Atrium_Roof",
            "GG_Blue_Room",
            "GG_Boss_Door_Entrance",
            "GG_End_Sequence",
            "GG_End_Sequence",
            "GG_Engine",
            "GG_Engine_Prime",
            "GG_Engine_Root",
            "GG_Spa",
            "GG_Unn",
            "GG_Workshop",
            "GG_Wyrm",
        };

        private static readonly Dictionary<string, int> BossSceneToKillsRequiredToWin = new Dictionary<string, int>
        {
            //[INFO]:[GodhomeWinLossTracker] - OnSceneLoad: GG_Vengefly_V
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Scene changed to GG_Vengefly_V
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss changed to Vengefly King (GG_Vengefly_V)
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Sequence changed to HoG
            //[INFO]:[GodhomeWinLossTracker] - OnEndBossScene
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss died
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Won Vengefly King in HoG
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Won Vengefly King in HoG (registered)
            //[INFO]:[GodhomeWinLossTracker] - OnSceneLoad: GG_Workshop
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Scene changed to GG_Workshop
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss changed to null
            { "GG_Vengefly_V", 1 },
            //[INFO]:[GodhomeWinLossTracker] - OnSceneLoad: GG_Nailmasters
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Scene changed to GG_Nailmasters
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss changed to Brothers Oro & Mato (GG_Nailmasters)
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Sequence changed to HoG
            //[INFO]:[GodhomeWinLossTracker] - OnEndBossScene
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss died // 2nd phase
            //[INFO]:[GodhomeWinLossTracker] - OnEndBossScene
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss died // 2nd phase
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Won Brothers Oro & Mato in HoG
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Won Brothers Oro & Mato in HoG (registered)
            //[INFO]:[GodhomeWinLossTracker] - OnSceneLoad: GG_Workshop
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Scene changed to GG_Workshop
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss changed to null
            { "GG_Nailmasters", 2 },
            //[INFO]:[GodhomeWinLossTracker] - OnSceneLoad: GG_Oblobbles
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Scene changed to GG_Oblobbles
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss changed to Oblobbles (GG_Oblobbles)
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Sequence changed to HoG
            //[INFO]:[GodhomeWinLossTracker] - OnEndBossScene
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss died
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Won Oblobbles in HoG
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Won Oblobbles in HoG (registered)
            //[INFO]:[GodhomeWinLossTracker] - OnSceneLoad: GG_Workshop
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Scene changed to GG_Workshop
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss changed to null
            { "GG_Oblobbles", 1 },
            //[INFO]:[GodhomeWinLossTracker] - OnSceneLoad: GG_Mantis_Lords
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Scene changed to GG_Mantis_Lords
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss changed to Mantis Lords (GG_Mantis_Lords)
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Sequence changed to HoG
            //[INFO]:[GodhomeWinLossTracker] - OnEndBossScene
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss died
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Won Mantis Lords in HoG
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Won Mantis Lords in HoG (registered)
            //[INFO]:[GodhomeWinLossTracker] - OnSceneLoad: GG_Workshop
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Scene changed to GG_Workshop
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss changed to null
            { "GG_Mantis_Lords", 1 },
            //[INFO]:[GodhomeWinLossTracker] - OnSceneLoad: GG_Mantis_Lords_V
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Scene changed to GG_Mantis_Lords_V
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss changed to Sisters of Battle (GG_Mantis_Lords_V)
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Sequence changed to HoG
            //[INFO]:[GodhomeWinLossTracker] - OnEndBossScene
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss died
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Won Sisters of Battle in HoG
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Won Sisters of Battle in HoG (registered)
            //[INFO]:[GodhomeWinLossTracker] - OnSceneLoad: GG_Workshop
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Scene changed to GG_Workshop
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss changed to null
            { "GG_Mantis_Lords_V", 1 },
            //[INFO]:[GodhomeWinLossTracker] - OnSceneLoad: GG_God_Tamer
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Scene changed to GG_God_Tamer
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss changed to God Tamer (GG_God_Tamer)
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Sequence changed to HoG
            //[INFO]:[GodhomeWinLossTracker] - OnEndBossScene
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss died
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Won God Tamer in HoG
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Won God Tamer in HoG (registered)
            //[INFO]:[GodhomeWinLossTracker] - OnSceneLoad: GG_Workshop
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Scene changed to GG_Workshop
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss changed to null
            { "GG_God_Tamer", 1 },
            //[INFO]:[GodhomeWinLossTracker] - OnSceneLoad: GG_Watcher_Knights
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Scene changed to GG_Watcher_Knights
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss changed to Watcher Knight (GG_Watcher_Knights)
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Sequence changed to HoG
            //[INFO]:[GodhomeWinLossTracker] - OnEndBossScene
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss died
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Won Watcher Knight in HoG
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Won Watcher Knight in HoG (registered)
            //[INFO]:[GodhomeWinLossTracker] - OnSceneLoad: GG_Workshop
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Scene changed to GG_Workshop
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss changed to null
            { "GG_Watcher_Knights", 1 },
            //[INFO]:[GodhomeWinLossTracker] - OnSceneLoad: GG_Radiance
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Scene changed to GG_Radiance
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss changed to Absolute Radiance (GG_Radiance)
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Sequence changed to HoG
            //[INFO]:[GodhomeWinLossTracker] - OnEndBossScene
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss died
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Won Absolute Radiance in HoG
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Won Absolute Radiance in HoG (registered)
            //[INFO]:[GodhomeWinLossTracker] - OnSceneLoad: GG_Workshop
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Scene changed to GG_Workshop
            //[INFO]:[GodhomeWinLossTracker] - Message on bus: Boss changed to null
            { "GG_Radiance", 1 },
        };

        private static readonly string[][] PantheonBossSceneNames = new string[][] {
            // p1
            new string[] {
                "GG_Vengefly",
                "GG_Gruz_Mother",
                "GG_False_Knight",
                "GG_Mega_Moss_Charger",
                "GG_Hornet_1",
                "GG_Ghost_Gorb",
                "GG_Dung_Defender",
                "GG_Mage_Knight",
                "GG_Brooding_Mawlek",
                "GG_Nailmasters",
            },
            // p2
            new string[] {
                "GG_Ghost_Xero"      ,
                "GG_Crystal_Guardian",
                "GG_Soul_Master"     ,
                "GG_Oblobbles"       ,
                "GG_Mantis_Lords"    ,
                "GG_Ghost_Marmu"     ,
                "GG_Nosk"            ,
                "GG_Flukemarm"       ,
                "GG_Broken_Vessel"   ,
                "GG_Painter"         ,
            },
            // p3
            new string[]
            {
                "GG_Hive_Knight"     ,
                "GG_Ghost_Hu"        ,
                "GG_Collector"       ,
                "GG_God_Tamer"       ,
                "GG_Grimm"           ,
                "GG_Ghost_Galien"    ,
                "GG_Grey_Prince_Zote",
                "GG_Uumuu"           ,
                "GG_Hornet_2"        ,
                "GG_Sly"             ,
            },
            // p4
            new string[]
            {
                "GG_Crystal_Guardian_2",
                "GG_Lost_Kin"          ,
                "GG_Ghost_No_Eyes"     ,
                "GG_Traitor_Lord"      ,
                "GG_White_Defender"    ,
                "GG_Failed_Champion"   ,
                "GG_Ghost_Markoth"     ,
                "GG_Watcher_Knights"   ,
                "GG_Soul_Tyrant"       ,
                "GG_Hollow_Knight"     ,
            },
            // p5
            new string[]
            {
                "GG_Vengefly_V"        ,
                "GG_Gruz_Mother_V"     ,
                "GG_False_Knight"      ,
                "GG_Mega_Moss_Charger" ,
                "GG_Hornet_1"          ,
                "GG_Ghost_Gorb_V"      ,
                "GG_Dung_Defender"     ,
                "GG_Mage_Knight_V"     ,
                "GG_Brooding_Mawlek_V" ,
                "GG_Nailmasters"       ,
                "GG_Ghost_Xero_V"      ,
                "GG_Crystal_Guardian"  ,
                "GG_Soul_Master"       ,
                "GG_Oblobbles"         ,
                "GG_Mantis_Lords_V"    ,
                "GG_Ghost_Marmu_V"     ,
                "GG_Flukemarm"         ,
                "GG_Broken_Vessel"     ,
                "GG_Ghost_Galien"      ,
                "GG_Painter"           ,
                "GG_Hive_Knight"       ,
                "GG_Ghost_Hu"          ,
                "GG_Collector_V"       ,
                "GG_God_Tamer"         ,
                "GG_Grimm"             ,
                "GG_Watcher_Knights"   ,
                "GG_Uumuu_V"           ,
                "GG_Nosk_Hornet"       ,
                "GG_Sly"               ,
                "GG_Hornet_2"          ,
                "GG_Crystal_Guardian_2",
                "GG_Lost_Kin"          ,
                "GG_Ghost_No_Eyes_V"   ,
                "GG_Traitor_Lord"      ,
                "GG_White_Defender"    ,
                "GG_Soul_Tyrant"       ,
                "GG_Ghost_Markoth_V"   ,
                "GG_Grey_Prince_Zote"  ,
                "GG_Failed_Champion"   ,
                "GG_Grimm_Nightmare"   ,
                "GG_Hollow_Knight"     ,
                "GG_Radiance"          ,
            },
        };

        private static readonly Dictionary<string, string> BossSceneToName = new()
        {
            { "GG_Broken_Vessel", "Broken Vessel" },
            { "GG_Brooding_Mawlek", "Brooding Mawlek" },
            { "GG_Brooding_Mawlek_V", "Brooding Mawlek" },
            { "GG_Collector", "The Collector" },
            { "GG_Collector_V", "The Collector" },
            { "GG_Crystal_Guardian", "Crystal Guardian" },
            { "GG_Crystal_Guardian_2", "Enraged Guardian" },
            { "GG_Dung_Defender", "Dung Defender" },
            { "GG_Failed_Champion", "Failed Champion" },
            { "GG_False_Knight", "False Knight" },
            { "GG_Flukemarm", "Flukemarm" },
            { "GG_Ghost_Galien", "Galien" },
            { "GG_Ghost_Gorb", "Gorb" },
            { "GG_Ghost_Gorb_V", "Gorb" },
            { "GG_Ghost_Hu", "Elder Hu" },
            { "GG_Ghost_Markoth", "Markoth" },
            { "GG_Ghost_Markoth_V", "Markoth" },
            { "GG_Ghost_Marmu", "Marmu" },
            { "GG_Ghost_Marmu_V", "Marmu" },
            { "GG_Ghost_No_Eyes", "No Eyes" },
            { "GG_Ghost_No_Eyes_V", "No Eyes" },
            { "GG_Ghost_Xero", "Xero" },
            { "GG_Ghost_Xero_V", "Xero" },
            { "GG_God_Tamer", "God Tamer" },
            { "GG_Grey_Prince_Zote", "Grey Prince Zote" },
            { "GG_Grimm", "Troupe Master Grimm" },
            { "GG_Grimm_Nightmare", "Nightmare King Grimm" },
            { "GG_Gruz_Mother", "Gruz Mother" },
            { "GG_Gruz_Mother_V", "Gruz Mother" },
            { "GG_Hive_Knight", "Hive Knight" },
            { "GG_Hollow_Knight", "Pure Vessel" },
            { "GG_Hornet_1", "Hornet (Protector)" },
            { "GG_Hornet_2", "Hornet (Sentinel)" },
            { "GG_Lost_Kin", "Lost Kin" },
            { "GG_Mage_Knight", "Soul Warrior" },
            { "GG_Mage_Knight_V", "Soul Warrior" },
            { "GG_Mantis_Lords", "Mantis Lords" },
            { "GG_Mantis_Lords_V", "Sisters of Battle" },
            { "GG_Mega_Moss_Charger", "Massive Moss Charger" },
            { "GG_Nailmasters", "Brothers Oro & Mato" },
            { "GG_Nosk", "Nosk" },
            { "GG_Nosk_Hornet", "Winged Nosk" },
            { "GG_Oblobbles", "Oblobbles" },
            { "GG_Painter", "Paintmaster Sheo" },
            { "GG_Radiance", "Absolute Radiance" },
            { "GG_Sly", "Great Nailsage Sly" },
            { "GG_Soul_Master", "Soul Master" },
            { "GG_Soul_Tyrant", "Soul Tyrant" },
            { "GG_Traitor_Lord", "Traitor Lord" },
            { "GG_Uumuu", "Uumuu" },
            { "GG_Uumuu_V", "Uumuu" },
            { "GG_Vengefly", "Vengefly King" },
            { "GG_Vengefly_V", "Vengefly King" },
            { "GG_Watcher_Knights", "Watcher Knight" },
            { "GG_White_Defender", "White Defender" },
        };
    }
}
