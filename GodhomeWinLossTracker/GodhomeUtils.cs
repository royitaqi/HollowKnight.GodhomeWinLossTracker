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

            // Remove the "GG_" prefix.
            if (sceneName.StartsWith("GG_"))
            {
                sceneName = sceneName.Substring(3);
            }
            // Remove the potential "_V" suffix.
            // This suffix appears for some ascended HoG boss fights which has special arena and/or minions (e.g. The Collector, No Eyes, Soul Warrior, etc).
            if (sceneName.EndsWith("_V"))
            {
                sceneName = sceneName.Substring(0, sceneName.Length - 2);
            }
            // Make boss name nicer to look at.
            return sceneName.Replace("_", " ");
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
    }
}
