using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class BusEvent : IMessage
    {
        public string Event { get; set; }

        public override string ToString()
        {
            string displayEvent = Event ?? "null";
            return $"Bus event: {displayEvent}";
        }
    }
}
