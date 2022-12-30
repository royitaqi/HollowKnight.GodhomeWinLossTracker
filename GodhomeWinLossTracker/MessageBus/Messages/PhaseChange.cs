using System.Diagnostics;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class PhaseChange : IMessage
    {
        public PhaseChange() : this(0) { }

        public PhaseChange(int phase)
        {
            DevUtils.Assert(phase >= 0 && phase <= 6, "Phase should be within 0..6");

            Phase = phase;
        }

        public int Phase { get; private set; }

        public override string ToString()
        {
            return $"Phase changed to {Phase}";
        }
    }
}
