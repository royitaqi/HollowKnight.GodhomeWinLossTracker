using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class HoGStatsQuery : IMessage
    {
        public HoGStatsQuery(string hogNameKey, Action<string> callback)
        {
            BossName = GodhomeUtils.GetNullableBossNameByHoGNameKey(hogNameKey);
            DevUtils.Assert(BossName != null, "hogNameKey should be a valid HoG name key");
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
