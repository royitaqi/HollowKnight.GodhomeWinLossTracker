using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    // BusEvent should be completely deletable for the bus to run properly.
    // It's here for dev/debug purpose.
    internal class BusEvent : IMessage
    {
        public string Event { get; set; }
        public bool ForTest { get; set; }

        public override string ToString()
        {
            string eventType = ForTest ? "Test" : "Bus";
            string eventString = Event ?? "null";
            return $"{eventType} event: {eventString}";
        }
    }
}
