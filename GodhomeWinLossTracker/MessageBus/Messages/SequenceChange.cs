using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class SequenceChange : IMessage
    {
        public string Name { get; set; }

        public override string ToString()
        {
            string displayName = Name ?? "null";
            return $"Sequence changed to {displayName}";
        }
    }
}
