using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GodhomeWinLossTracker.Utils
{
    internal static class GodhomeUtils
    {
        internal static bool IsBossName(string bossName)
        {
            return BossSceneToCanonicalName.Any(kvp => kvp.Value == bossName);
        }

        internal static bool IsBossScene(string sceneName)
        {
            return BossSceneToCanonicalName.ContainsKey(sceneName);
        }

        internal static bool IsNonBossScene(string sceneName)
        {
            return !BossSceneToCanonicalName.ContainsKey(sceneName);
        }

        internal static string GetNullableBossNameBySceneName(string sceneName)
        {
            // Return null for non-boss scene
            if (IsNonBossScene(sceneName))
            {
                return null;
            }

            DevUtils.Assert(BossSceneToCanonicalName.ContainsKey(sceneName), $"Boss scene name {sceneName} should exist in BossSceneToName");
            return BossSceneToCanonicalName[sceneName];
        }

        internal static string GetNullableBossNameByHoGNameKey(string hogNameKey)
        {
            return BossInfos.FirstOrDefault(info => info.HoGNameKey == hogNameKey)?.CanonicalName;
        }

        [Flags]
        public enum PantheonAttributes
        {
            None = 0,
            IsSegment = 1,
        }

        internal static string GetPantheonSequenceName(int? pantheonIndex, PantheonAttributes attributes)
        {
            // No index, no name.
            if (pantheonIndex == null)
            {
                return null;
            }

            string name = $"P{pantheonIndex + 1}";
            if (attributes == PantheonAttributes.IsSegment)
            {
                name += "/Seg";
            }
            return name;
        }

        private static int? GetPantheonIndex(string previousSceneName, string bossSceneName)
        {
            DevUtils.Assert(IsBossScene(bossSceneName), "bossSceneName should be passed in valid, i.e. a boss scene");

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
            DevUtils.Assert(IsBossScene(bossSceneName), "bossSceneName should be passed in valid, i.e. a boss scene");

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
            DevUtils.Assert(IsBossScene(bossSceneName), "bossSceneName should be passed in valid, i.e. a boss scene");
            if (BossSceneToKillsRequiredToWin.ContainsKey(bossSceneName))
            {
                return BossSceneToKillsRequiredToWin[bossSceneName];
            }
            else
            {
                return 1;
            }
        }

        internal static IEnumerable<string> GetBossScenesByName(string bossName)
        {
            return BossSceneToCanonicalName.Where(kvp => kvp.Value == bossName).Select(kvp => kvp.Key);
        }

        internal static int? GetPantheonIndexFromDescriptionKey(string descriptionKey)
        {
            int ret = PantheonDescriptionKeys.IndexOf(descriptionKey);
            return ret >= 0 ? ret : null;
        }

        internal static IEnumerable<string> GetPantheonScenes(int index)
        {
            DevUtils.Assert(index < 5, "Pantheon index can be from 0 to 4");
            return PantheonBossSceneNames[index];
        }

        internal static int GetBossInitialPhase(string bossScene)
        {
            if (!BossPhases.ContainsKey(bossScene))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        internal static int GetBossPhase(string bossScene, int maxHP, int hp)
        {
            if (!BossPhases.ContainsKey(bossScene))
            {
                return 0;
            }

            int[][] configs = BossPhases[bossScene];
            var config = configs.FirstOrDefault(config => config[0] == maxHP);
            if (config == null)
            {
                GodhomeWinLossTracker.instance?.LogModWarn($"BossPhases data contains boss {bossScene} but has no matching configuration for max HP {maxHP}");
                return 0;
            }

            int phase;
            for (phase = 1; phase < config.Length; phase++)
            {
                if (hp > config[phase])
                {
                    break;
                }
            }
            return phase;
        }

        internal static readonly Dictionary<string, int[][]> BossPhases = new()
        {
            { "GG_Radiance", new[] {
                new[] {
                    2280, // 1
                    2280 - 400, // 2 = 1880
                    2280 - 400 - 450, // 3 = 1430
                    2280 - 400 - 450 - 300, // 4 = 1130
                    2280 - 400 - 450 - 300 - 750, // 5 = 380
                    280, // 6
                    int.MinValue,
                },
            } },
        };

        internal static readonly List<string> PantheonDescriptionKeys = new()
        {
            "UI_CHALLENGE_DESC_1",
            "UI_CHALLENGE_DESC_2",
            "UI_CHALLENGE_DESC_3",
            "UI_CHALLENGE_DESC_4",
            "UI_CHALLENGE_DESC_5",
        };

        internal static readonly Dictionary<string, int> BossSceneToKillsRequiredToWin = new()
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

        internal static readonly string[][] PantheonBossSceneNames = new string[][] {
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

        internal static readonly Dictionary<string, string> BossSceneToCanonicalName = new()
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

        internal class BossInfo
        {
            public string HoGNameKey { get; set; }
            public string HoGEnglishName { get; set; }
            public string HoGDescriptionKey { get; set; }
            public string HoGEnglishDescription { get; set; }
            public string CanonicalName { get; set; }
        }

        internal static readonly BossInfo[] BossInfos = new[] {
            new BossInfo {
                CanonicalName = "Gruz Mother",
                HoGNameKey = "NAME_BIGFLY",
                HoGEnglishName = "Gruz Mother",
                HoGDescriptionKey ="GG_S_GRUZ",
                HoGEnglishDescription = "Slumbering god of fertility",
            },
            new BossInfo {
                CanonicalName = "Vengefly King",
                HoGNameKey = "NAME_BIGBUZZER",
                HoGEnglishName = "Vengefly King",
                HoGDescriptionKey ="GG_S_BIGBUZZ",
                HoGEnglishDescription = "Vicious god of territories",
            },
            new BossInfo {
                CanonicalName = "Brooding Mawlek",
                HoGNameKey = "NAME_MAWLEK",
                HoGEnglishName = "Brooding Mawlek",
                HoGDescriptionKey ="GG_S_MAWLEK",
                HoGEnglishDescription = "Lonely god of the nest",
            },
            new BossInfo {
                CanonicalName = "False Knight",
                HoGNameKey = "NAME_FALSEKNIGHT",
                HoGEnglishName = "False Knight",
                HoGDescriptionKey ="GG_S_FKNIGHT",
                HoGEnglishDescription = "Angry god of the downtrodden",
            },
            new BossInfo {
                CanonicalName = "Failed Champion",
                HoGNameKey = "NAME_FAILED_CHAMPION",
                HoGEnglishName = "Failed Champion",
                HoGDescriptionKey ="GG_S_FAILED_CHAMPION",
                HoGEnglishDescription = "Baleful god of regrets",
            },
            new BossInfo {
                CanonicalName = "Hornet (Protector)",
                HoGNameKey = "NAME_HORNET_1",
                HoGEnglishName = "Hornet Protector",
                HoGDescriptionKey ="GG_S_HORNET",
                HoGEnglishDescription = "God protector of a fading land",
            },
            new BossInfo {
                CanonicalName = "Hornet (Sentinel)",
                HoGNameKey = "NAME_HORNET_2",
                HoGEnglishName = "Hornet Sentinel",
                HoGDescriptionKey ="GG_S_HORNET",
                HoGEnglishDescription = "God protector of a fading land",
            },
            new BossInfo {
                CanonicalName = "Massive Moss Charger",
                HoGNameKey = "NAME_MEGA_MOSS_CHARGER",
                HoGEnglishName = "Massive Moss Charger",
                HoGDescriptionKey ="GG_S_MEGAMOSS",
                HoGEnglishDescription = "Restless god of those who band together",
            },
            new BossInfo {
                CanonicalName = "Flukemarm",
                HoGNameKey = "NAME_FLUKE_MOTHER",
                HoGEnglishName = "Flukemarm",
                HoGDescriptionKey ="GG_S_FLUKEMUM",
                HoGEnglishDescription = "Alluring God of motherhood",
            },
            new BossInfo {
                CanonicalName = "Mantis Lords",
                HoGNameKey = "NAME_MANTIS_LORD",
                HoGEnglishName = "Mantis Lords",
                HoGDescriptionKey ="GG_S_MANTISLORDS",
                HoGEnglishDescription = "Noble sister gods of combat",
            },
            new BossInfo {
                CanonicalName = "Sisters of Battle",
                HoGNameKey = "NAME_MANTIS_LORD_V",
                HoGEnglishName = "Sisters of Battle",
                HoGDescriptionKey ="GG_S_MANTIS_LORD_V",
                HoGEnglishDescription = "Revered gods of a proud tribe",
            },
            new BossInfo {
                CanonicalName = "Oblobbles",
                HoGNameKey = "NAME_OBLOBBLE",
                HoGEnglishName = "Oblobble",
                HoGDescriptionKey ="GG_S_BIGBEES",
                HoGEnglishDescription = "Lover gods of faith and devotion",
            },
            new BossInfo {
                CanonicalName = "Hive Knight",
                HoGNameKey = "NAME_HIVE_KNIGHT",
                HoGEnglishName = "Hive Knight",
                HoGDescriptionKey ="GG_S_HIVEKNIGHT",
                HoGEnglishDescription = "Watchful god of duty",
            },
            new BossInfo {
                CanonicalName = "Broken Vessel",
                HoGNameKey = "NAME_INFECTED_KNIGHT",
                HoGEnglishName = "Broken Vessel",
                HoGDescriptionKey ="GG_S_BROKENVESSEL",
                HoGEnglishDescription = "Broken shell of an empty god",
            },
            new BossInfo {
                CanonicalName = "Lost Kin",
                HoGNameKey = "NAME_LOST_KIN",
                HoGEnglishName = "Lost Kin",
                HoGDescriptionKey ="GG_S_LOST_KIN",
                HoGEnglishDescription = "Lost god of the Abyss",
            },
            new BossInfo {
                CanonicalName = "Nosk",
                HoGNameKey = "NAME_MIMIC_SPIDER",
                HoGEnglishName = "Nosk",
                HoGDescriptionKey ="GG_S_NOSK",
                HoGEnglishDescription = "Everchanging god of the faceless",
            },
            new BossInfo {
                CanonicalName = "Winged Nosk",
                HoGNameKey = "NAME_NOSK_HORNET",
                HoGEnglishName = "Winged Nosk",
                HoGDescriptionKey ="GG_S_NOSK_HORNET",
                HoGEnglishDescription = "Deceptive god assuming a protector's form",
            },
            new BossInfo {
                CanonicalName = "The Collector",
                HoGNameKey = "NAME_JAR_COLLECTOR",
                HoGEnglishName = "The Collector",
                HoGDescriptionKey ="GG_S_COLLECTOR",
                HoGEnglishDescription = "Joyful god of protection",
            },
            new BossInfo {
                CanonicalName = "God Tamer",
                HoGNameKey = "NAME_LOBSTER_LANCER",
                HoGEnglishName = "God Tamer",
                HoGDescriptionKey ="GG_S_LOBSTERLANCER",
                HoGEnglishDescription = "Gallant god of the arena",
            },
            new BossInfo {
                CanonicalName = "Crystal Guardian",
                HoGNameKey = "NAME_MEGA_BEAM_MINER_1",
                HoGEnglishName = "Crystal Guardian",
                HoGDescriptionKey ="GG_S_MEGABEAMMINER",
                HoGEnglishDescription = "Shining god of greed",
            },
            new BossInfo {
                CanonicalName = "Enraged Guardian",
                HoGNameKey = "NAME_MEGA_BEAM_MINER_2",
                HoGEnglishName = "Enraged Guardian",
                HoGDescriptionKey ="GG_S_MEGABEAMMINER",
                HoGEnglishDescription = "Shining god of greed",
            },
            new BossInfo {
                CanonicalName = "Uumuu",
                HoGNameKey = "NAME_MEGA_JELLYFISH",
                HoGEnglishName = "Uumuu",
                HoGDescriptionKey ="GG_S_UMMU",
                HoGEnglishDescription = "Uncanny god of knowledge",
            },
            new BossInfo {
                CanonicalName = "Traitor Lord",
                HoGNameKey = "NAME_TRAITOR_LORD",
                HoGEnglishName = "Traitor Lord",
                HoGDescriptionKey ="GG_S_TRAITORLORD",
                HoGEnglishDescription = "Treacherous god of anger",
            },
            new BossInfo {
                CanonicalName = "Grey Prince Zote",
                HoGNameKey = "NAME_GREY_PRINCE",
                HoGEnglishName = "Grey Prince Zote",
                HoGDescriptionKey ="GG_S_MIGHTYZOTE",
                HoGEnglishDescription = "False god conjured by the lonely",
            },
            new BossInfo {
                CanonicalName = "Soul Warrior",
                HoGNameKey = "NAME_MAGE_KNIGHT",
                HoGEnglishName = "Soul Warrior",
                HoGDescriptionKey ="GG_S_MAGEKNIGHT",
                HoGEnglishDescription = "Haunted god of the sanctum",
            },
            new BossInfo {
                CanonicalName = "Soul Master",
                HoGNameKey = "NAME_MAGE_LORD",
                HoGEnglishName = "Soul Master",
                HoGDescriptionKey ="GG_S_SOULMASTER",
                HoGEnglishDescription = "Covetous god of soul",
            },
            new BossInfo {
                CanonicalName = "Soul Tyrant",
                HoGNameKey = "NAME_SOUL_TYRANT",
                HoGEnglishName = "Soul Tyrant",
                HoGDescriptionKey ="GG_S_SOUL_TYRANT",
                HoGEnglishDescription = "Frenzied god of mortality",
            },
            new BossInfo {
                CanonicalName = "Dung Defender",
                HoGNameKey = "NAME_DUNG_DEFENDER",
                HoGEnglishName = "Dung Defender",
                HoGDescriptionKey ="GG_S_DUNGDEF",
                HoGEnglishDescription = "Kindly god of bravery and honour",
            },
            new BossInfo {
                CanonicalName = "White Defender",
                HoGNameKey = "NAME_WHITE_DEFENDER",
                HoGEnglishName = "White Defender",
                HoGDescriptionKey ="GG_S_DUNGDEF",
                HoGEnglishDescription = "Kindly god of bravery and honour",
            },
            new BossInfo {
                CanonicalName = "Watcher Knight",
                HoGNameKey = "NAME_BLACK_KNIGHT",
                HoGEnglishName = "Watcher Knight",
                HoGDescriptionKey ="GG_S_WATCHERKNIGHTS",
                HoGEnglishDescription = "Sentinel gods of the spire",
            },
            new BossInfo {
                CanonicalName = "No Eyes",
                HoGNameKey = "NAME_GHOST_NOEYES",
                HoGEnglishName = "No Eyes",
                HoGDescriptionKey ="GG_S_GHOST_NOEYES",
                HoGEnglishDescription = "Dreamborn god of fear and relief",
            },
            new BossInfo {
                CanonicalName = "Marmu",
                HoGNameKey = "NAME_GHOST_MARMU",
                HoGEnglishName = "Marmu",
                HoGDescriptionKey ="GG_S_GHOST_MARMU",
                HoGEnglishDescription = "Dreamborn god of gardens",
            },
            new BossInfo {
                CanonicalName = "Galien",
                HoGNameKey = "NAME_GHOST_GALIEN",
                HoGEnglishName = "Galien",
                HoGDescriptionKey ="GG_S_GHOST_GALIEN",
                HoGEnglishDescription = "Dreamborn god of heroic hearts",
            },
            new BossInfo {
                CanonicalName = "Markoth",
                HoGNameKey = "NAME_GHOST_MARKOTH",
                HoGEnglishName = "Markoth",
                HoGDescriptionKey ="GG_S_GHOST_MARKOTH",
                HoGEnglishDescription = "Dreamborn god of meditation and isolation",
            },
            new BossInfo {
                CanonicalName = "Xero",
                HoGNameKey = "NAME_GHOST_XERO",
                HoGEnglishName = "Xero",
                HoGDescriptionKey ="GG_S_GHOST_XERO",
                HoGEnglishDescription = "Dreamborn god of faith and betrayal",
            },
            new BossInfo {
                CanonicalName = "Gorb",
                HoGNameKey = "NAME_GHOST_ALADAR",
                HoGEnglishName = "Gorb",
                HoGDescriptionKey ="GG_S_GHOST_GORB",
                HoGEnglishDescription = "Dreamborn god of the beyond",
            },
            new BossInfo {
                CanonicalName = "Elder Hu",
                HoGNameKey = "NAME_GHOST_HU",
                HoGEnglishName = "Elder Hu",
                HoGDescriptionKey ="GG_S_GHOST_HU",
                HoGEnglishDescription = "Dreamborn god of travellers and sages",
            },
            new BossInfo {
                CanonicalName = "Brothers Oro & Mato",
                HoGNameKey = "NAME_NAILMASTERS",
                HoGEnglishName = "Oro & Mato",
                HoGDescriptionKey ="GG_S_NAILMASTER",
                HoGEnglishDescription = "Loyal brother gods of the nail",
            },
            new BossInfo {
                CanonicalName = "Paintmaster Sheo",
                HoGNameKey = "NAME_PAINTMASTER",
                HoGEnglishName = "Paintmaster Sheo",
                HoGDescriptionKey ="GG_S_PAINTMASTER",
                HoGEnglishDescription = "Talented god of artists and creators",
            },
            new BossInfo {
                CanonicalName = "Great Nailsage Sly",
                HoGNameKey = "NAME_SLY",
                HoGEnglishName = "Nailsage Sly",
                HoGDescriptionKey ="GG_S_SLY",
                HoGEnglishDescription = "Cunning god of opportunity",
            },
            new BossInfo {
                CanonicalName = "Pure Vessel",
                HoGNameKey = "NAME_HK_PRIME",
                HoGEnglishName = "Pure Vessel",
                HoGDescriptionKey ="GG_S_HK",
                HoGEnglishDescription = "Mighty god of nothingness",
            },
            new BossInfo {
                CanonicalName = "Troupe Master Grimm",
                HoGNameKey = "NAME_GRIMM",
                HoGEnglishName = "Grimm",
                HoGDescriptionKey ="GG_S_GRIM",
                HoGEnglishDescription = "Travelling god of the troupe",
            },
            new BossInfo {
                CanonicalName = "Nightmare King Grimm",
                HoGNameKey = "NAME_NIGHTMARE_GRIMM",
                HoGEnglishName = "Nightmare King",
                HoGDescriptionKey ="GG_S_NIGHTMARE_KING",
                HoGEnglishDescription = "God of nightmares",
            },
            new BossInfo {
                CanonicalName = "Absolute Radiance",
                HoGNameKey = "NAME_FINAL_BOSS",
                HoGEnglishName = "Radiance",
                HoGDescriptionKey ="GG_S_RADIANCE",
                HoGEnglishDescription = "Forgotten god of light",
            },
        };

        internal static string GetBossFsm(string bossGoName)
        {
            if (BossGoNameToFsm.ContainsKey(bossGoName))
            {
                return BossGoNameToFsm[bossGoName];
            }
            else
            {
                return null;
            }
        }

        internal static readonly Dictionary<string, string> BossGoNameToFsm = new()
        {
            { "Absolute Radiance", "Attack Commands" },
        };

        internal static DamageSourceFsm[] GetBossDamageSourceFsms(string bossGoName)
        {
            if (BossGoNameToDamageSourceFsms.ContainsKey(bossGoName))
            {
                return BossGoNameToDamageSourceFsms[bossGoName];
            }
            else
            {
                return null;
            }
        }

        internal struct DamageSourceFsm
        {
            public string FsmName;
            public string StateName;
            public string VariableName;
            // The index of where to insert the tagging logic.
            // If unspecified, append at the end.
            public int? ActionIndex;
            public string DamageSource;
            public string DamageSourceDetail;
        }

        // This will cover two scenarios:
        // 1) Any damage source that is a descendant of the boss GO. See GoTagExtension where the root lookup is implemented. E.g. Mage Knight's dash and stomp.
        // 2) Any damage source that is NOT a descendant of the boss GO but the same damage source is used in different boss attacks. E.g. AbsRad's different sword attacks.
        internal static readonly Dictionary<string, DamageSourceFsm[]> BossGoNameToDamageSourceFsms = new()
        {
            {
                "Mage Knight", // Soul Warrior
                new[]
                {
                    // Stomp
                    new DamageSourceFsm
                    {
                        FsmName = "Mage Knight",
                        StateName = "Up Tele",
                        VariableName = null, // Soul Warrior itself
                        DamageSource = "Stomp",
                        DamageSourceDetail = "Up Tele",
                    },
                    new DamageSourceFsm
                    {
                        FsmName = "Mage Knight",
                        StateName = "Stomp Recover",
                        VariableName = null, // Soul Warrior itself
                        DamageSource = null, // Clear GoTag
                    },
                    // Dash
                    new DamageSourceFsm
                    {
                        FsmName = "Mage Knight",
                        StateName = "Slash Aim",
                        VariableName = null, // Soul Warrior itself
                        DamageSource = "Dash",
                        DamageSourceDetail = "Slash Aim",
                    },
                    new DamageSourceFsm
                    {
                        FsmName = "Mage Knight",
                        StateName = "Slash Recover",
                        VariableName = null, // Soul Warrior itself
                        DamageSource = null, // Clear GoTag
                    },
                }
            },
            {
                "HK Prime", // Pure Vessel
                new[]
                {
                    // Triple Slash (SLASH)
                    new DamageSourceFsm
                    {
                        FsmName = "Control",
                        StateName = "Slash1 Antic",
                        VariableName = null, // PV itself
                        DamageSource = "Triple Slash",
                        DamageSourceDetail = "Slash1 Antic",
                    },
                    new DamageSourceFsm
                    {
                        FsmName = "Control",
                        StateName = "Slash3 Recover",
                        VariableName = null, // PV itself
                        DamageSource = null, // Clear GoTag
                    },
                    // Stomp + Floor Spike (DSTAB) (generates HK Plume Prime)
                    new DamageSourceFsm
                    {
                        FsmName = "Control",
                        StateName = "Stomp Antic",
                        VariableName = null, // PV itself
                        DamageSource = "Stomp",
                        DamageSourceDetail = "Stomp Antic",
                    },
                    new DamageSourceFsm
                    {
                        FsmName = "Control",
                        StateName = "Stomp Recover",
                        VariableName = null, // PV itself
                        DamageSource = null, // Clear GoTag
                    },
                    // Fan (SMALL SHOT) (generates Shot HK Shadow)
                    new DamageSourceFsm
                    {
                        FsmName = "Control",
                        StateName = "SmallShot Antic",
                        VariableName = null, // PV itself
                        DamageSource = "Fan",
                        DamageSourceDetail = "SmallShot Antic",
                    },
                    new DamageSourceFsm
                    {
                        FsmName = "Control",
                        StateName = "SmallShot Recover",
                        VariableName = null, // PV itself
                        DamageSource = null, // Clear GoTag
                    },
                    // Focus (FOCUS) (generates hero_damager)
                    new DamageSourceFsm
                    {
                        FsmName = "Control",
                        StateName = "Focus Charge",
                        VariableName = null, // PV itself
                        DamageSource = "Focus",
                        DamageSourceDetail = "Focus Charge",
                    },
                    new DamageSourceFsm
                    {
                        FsmName = "Control",
                        StateName = "Focus Wait",
                        VariableName = null, // PV itself
                        DamageSource = null, // Clear GoTag
                    },
                    // Dash (DASH)
                    new DamageSourceFsm
                    {
                        FsmName = "Control",
                        StateName = "Dash Antic",
                        VariableName = null, // PV itself
                        DamageSource = "Dash",
                        DamageSourceDetail = "Dash Antic",
                    },
                    new DamageSourceFsm
                    {
                        FsmName = "Control",
                        StateName = "Dash End",
                        VariableName = null, // PV itself
                        DamageSource = null, // Clear GoTag
                    },
                    // Tendril (TENDRIL)
                    new DamageSourceFsm
                    {
                        FsmName = "Control",
                        StateName = "Tendril Antic",
                        VariableName = null, // PV itself
                        DamageSource = "Tendril",
                        DamageSourceDetail = "Tendril Antic",
                    },
                    new DamageSourceFsm
                    {
                        FsmName = "Control",
                        StateName = "Tendril Recover",
                        VariableName = null, // PV itself
                        DamageSource = null, // Clear GoTag
                    },
                    // Parry (COUNTER)
                    new DamageSourceFsm
                    {
                        FsmName = "Control",
                        StateName = "Counter Antic",
                        VariableName = null, // PV itself
                        DamageSource = "Parry",
                        DamageSourceDetail = "Counter Antic",
                    },
                    new DamageSourceFsm
                    {
                        FsmName = "Control",
                        StateName = "Recollider",
                        VariableName = null, // PV itself
                        DamageSource = null, // Clear GoTag
                    },
                    new DamageSourceFsm
                    {
                        FsmName = "Control",
                        StateName = "CSlash Recover",
                        VariableName = null, // PV itself
                        DamageSource = null, // Clear GoTag
                    },
                }
            },
            {
                "Absolute Radiance",
                new[]
                {
                    // Face swords
                    new DamageSourceFsm
                    {
                        FsmName = "Attack Commands",
                        StateName = "CW Spawn",
                        VariableName = "Attack Obj",
                        DamageSource = "Face Swords",
                        DamageSourceDetail = "CW Spawn",
                    },
                    new DamageSourceFsm
                    {
                        FsmName = "Attack Commands",
                        StateName = "CCW Spawn",
                        VariableName = "Attack Obj",
                        DamageSource = "Face Swords",
                        DamageSourceDetail = "CCW Spawn",
                    },
                    // Sword rain
                    new DamageSourceFsm
                    {
                        FsmName = "Attack Commands",
                        StateName = "Comb Top",
                        VariableName = "Attack Obj",
                        DamageSource = "Sword Rain",
                        DamageSourceDetail = "Comb Top",
                    },
                    new DamageSourceFsm
                    {
                        FsmName = "Attack Commands",
                        StateName = "Comb Top 2",
                        VariableName = "Attack Obj",
                        DamageSource = "Sword Rain",
                        DamageSourceDetail = "Comb Top 2",
                    },
                    // Sword wall
                    new DamageSourceFsm
                    {
                        FsmName = "Attack Commands",
                        StateName = "Comb L",
                        VariableName = "Attack Obj",
                        DamageSource = "Sword Wall",
                        DamageSourceDetail = "Comb L",
                    },
                    new DamageSourceFsm
                    {
                        FsmName = "Attack Commands",
                        StateName = "Comb R",
                        VariableName = "Attack Obj",
                        DamageSource = "Sword Wall",
                        DamageSourceDetail = "Comb R",
                    },
                    new DamageSourceFsm
                    {
                        FsmName = "Attack Commands",
                        StateName = "Comb L 2",
                        VariableName = "Attack Obj",
                        DamageSource = "Sword Wall",
                        DamageSourceDetail = "Comb L 2",
                    },
                    new DamageSourceFsm
                    {
                        FsmName = "Attack Commands",
                        StateName = "Comb R 2",
                        VariableName = "Attack Obj",
                        DamageSource = "Sword Wall",
                        DamageSourceDetail = "Comb R 2",
                    },
                }
            },
        };

        /**
         * Cleans a damage source and maps it to a user-understandable description.
         * Returns the cleaned string if the damage source is unknown.
         */
        internal static string MapDamageSource(GameObject go)
        {
            // Walk up the parent tree for generic damage sources
            while (DamageSourceLookAtParent.Contains(go.name))
            {
                go = go.transform.parent.gameObject;
            }

            // Clean up damage source name
            string damageSource = go.name;
            int idx = damageSource.IndexOf('(');
            if (idx >= 0)
            {
                damageSource = damageSource.Substring(0, idx).Trim();
            }

            // Map damage source name if exists
            if (DamageSourceNameMap.ContainsKey(damageSource))
            {
                return DamageSourceNameMap[damageSource];
            }
            return damageSource;
        }

        // This should cover any damage source that is NOT a descendant of the boss GO AND can uniquely identify the boss attack.
        // To cover other damage sources, see BossGoNameToDamageSourceFsms.
        internal static readonly Dictionary<string, string> DamageSourceNameMap = new()
        {
            // All - orb attacks
            { "Hero Hurter", "Orb" },
            // PV
            { "Blast", "Focus" },
            //{ "Counter", "Parry" }, // migration
            //{ "Dash Stab", "Dash" }, // migration
            //{ "Dstab Damage", "Dash" }, // migration
            { "HK Plume Prime", "Floor Spike" },
            { "Shot HK Shadow", "Fan" },
            //{ "Slash Antic", "Parry" }, // migration
            //{ "Slash", "Parry" }, // migration
            //{ "Slash2", "Parry" }, // migration
            //{ "T Hit", "Tendril" }, // migration
            // AbsRad
            { "Cloud Hazard", "Fall" },
            { "Radiant Beam R", "Beam Wall" },
            { "Radiant Beam", "Face Beam" },
            { "Radiant Spike", "Floor Spike" },
            { "Spike Collider", "Fall" },
        };

        // A set of in-game damage sources which are so generic that they don't indicate a specific boss attack.
        // Their parents should be looked up to decide which boss attack caused the damage.
        internal static readonly HashSet<string> DamageSourceLookAtParent = new()
        {
            // PV: Battle Scene -> Focus Blasts -> HK Prime Blast -> Blast -> hero_damager
            "hero_damager",
        };
    }
}
