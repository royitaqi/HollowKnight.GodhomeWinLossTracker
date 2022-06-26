using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class PantheonStatsQuery : IMessage
    {
        public PantheonStatsQuery(string descriptionKey, Action<string /* runs */, string /* pb */, string /* churns */> callback)
        {
            DescriptionKey = descriptionKey;
            Callback = callback;
        }

        public override string ToString()
        {
            return $"Query pantheon stats for description key: {DescriptionKey}";
        }

        public Action<string, string, string> Callback { get; private set; }
        public string DescriptionKey { get; private set; }
    }
}
