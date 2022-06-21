using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    public class RawWinLoss : IMessage
    {
        public enum Sources : int
        {
            Manual = 0,
            Mod = 1,
        }

        public RawWinLoss(string sequenceName, string bossName, string sceneName, bool winLoss, long fightLengthMs, Sources source)
        {
            DevUtils.Assert(sequenceName != null, "sequenceName shouldn't be null");
            DevUtils.Assert(bossName != null, "bossName shouldn't be null");
            DevUtils.Assert(sceneName != null, "sceneName shouldn't be null");

            Timestamp = DateTime.Now;

            SequenceName = sequenceName;
            BossName = bossName;
            SceneName = sceneName;
            WinLoss = winLoss;
            FightLengthMs = fightLengthMs;
            Source = source;
        }

        public override string ToString()
        {
            string verb = WinLoss ? "Won" : "Lost to";
            return $"{verb} {BossName} in {SequenceName}";
        }

        public DateTime Timestamp { get; private set; }
        public string SequenceName { get; private set; }
        public string BossName { get; private set; }
        public string SceneName { get; private set; }
        public bool WinLoss { get; private set; }
        public long FightLengthMs { get; private set; }
        public Sources Source { get; private set; }
    }
}
