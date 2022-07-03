using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class TKHit: IMessage
    {
        public enum Types
        {
            Enemy = 1,
            Hazard = 2,
        }

        public int Damage { get; set; }
        public int HealthAfter { get; set; }
        public Types Type { get; set; }

        public override string ToString()
        {
            return $"TK took hit: Damage={Damage} HealthAfter={HealthAfter} Type={Type}";
        }
    }
}
