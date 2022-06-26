using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class HoGStatsQuery : IMessage
    {
        public HoGStatsQuery(string bossName, Action<string> callback)
        {
            DevUtils.Assert(GodhomeUtils.IsBossName(bossName), "bossName should be a valid boss name");

            BossName = bossName;
            Callback = callback;
        }

        public override string ToString()
        {
            return $"Query HoG stats: {BossName}";
        }

        public Action<string> Callback { get; private set; }
        public string BossName { get; private set; }
    }
}
