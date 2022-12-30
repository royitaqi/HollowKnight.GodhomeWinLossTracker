using System.Diagnostics;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class BossChange : IMessage
    {
        public BossChange() { }
        public BossChange(string bossName, string sceneName)
        {
            DevUtils.Assert(bossName != null, "bossName shouldn't be null");
            DevUtils.Assert(sceneName != null, "sceneName shouldn't be null");

            BossName = bossName;
            SceneName = sceneName;
        }

        public bool IsNoBoss() => BossName == null;
        public bool IsBoss() => BossName != null;

        public string BossName { get; private set; }
        public string SceneName { get; private set; }

        public override string ToString()
        {
            string displayName = IsNoBoss() ? "null" : $"{BossName} ({SceneName})";
            return $"Boss changed to {displayName}";
        }
    }
}
