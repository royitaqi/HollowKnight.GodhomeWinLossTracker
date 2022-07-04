using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class TKHeal: IMessage
    {
        public int Heal { get; set; }
        public int HealthAfter { get; set; }

        public override string ToString()
        {
            return $"TK healed: Heal={Heal} HealthAfter={HealthAfter}";
        }
    }
}
