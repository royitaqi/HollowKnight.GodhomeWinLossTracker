using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GodhomeWinLossTracker.MessageBus.Messages;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class BossSceneRecognizer: IHandler
    {
        public void OnMessage(TheMessageBus bus, Modding.Loggable logger, IMessage msg)
        {
            if (msg is SceneChange)
            {
                string sceneName = (msg as SceneChange).Name;
                Debug.Assert(sceneName != null);

                string bossName = GetPotentialBossName(sceneName);
                bus.Put(new BossChange { Name = bossName });
            }
        }

        private string GetPotentialBossName(string sceneName)
        {
            // If scene name is listed as non-boss, return null
            if (NonBossScenes.Contains(sceneName))
            {
                return null;
            }
            // Scene name is not listed, so it's a boss scene
            else
            {
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
    }
}
