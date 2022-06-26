using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class PantheonStatsQuery : IMessage
    {
        public PantheonStatsQuery(string pantheonName, Action<string, string, string> callback)
        {
            PantheonName = pantheonName;
            Callback = callback;
        }

        public override string ToString()
        {
            return $"Query pantheon stats: {PantheonName}";
        }

        public Action<string, string, string> Callback { get; private set; }
        public string PantheonName { get; private set; }
    }
}
