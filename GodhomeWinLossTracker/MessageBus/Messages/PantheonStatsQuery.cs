using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class PantheonStatsQuery : IMessage
    {
        public PantheonStatsQuery(int pantheonIndex, Action<string /* runs */, string /* pb */, string /* churns */> callback)
        {
            PantheonIndex = pantheonIndex;
            Callback = callback;
        }

        public override string ToString()
        {
            return $"Query pantheon stats for P{PantheonIndex + 1}";
        }

        public Action<string, string, string> Callback { get; private set; }
        public int PantheonIndex { get; private set; }
    }
}
