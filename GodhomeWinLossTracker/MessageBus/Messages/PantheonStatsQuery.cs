using System;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class PantheonStatsQuery : IMessage
    {

        public PantheonStatsQuery(int? pantheonIndex, GodhomeUtils.PantheonAttributes pantheonAttribute, Action<string /* runs */, string /* pb */, string /* churns */> callback)
        {
            PantheonIndex = pantheonIndex;
            PantheonAttribute = pantheonAttribute;
            Callback = callback;
        }

        public override string ToString()
        {
            string pantheon = PantheonIndex != null ? $"P{PantheonIndex + 1}" : "unidentified pantheon";
            return $"Query pantheon stats for {pantheon} with attributes {PantheonAttribute}";
        }

        // Null means unidentified sequence
        public int? PantheonIndex { get; private set; }
        public GodhomeUtils.PantheonAttributes PantheonAttribute { get; private set; }
        public Action<string, string, string> Callback { get; private set; }
    }
}
