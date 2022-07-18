using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    [ModLogLevel(Modding.LogLevel.Debug)]
    public class TKHit: IMessage
    {
        public enum DamageSources
        {
            Unknown = 0,
            Enemy = 1,
            Hazard = 2,
        }

        public int Damage { get; set; }
        public int HealthBefore => HealthAfter + Damage;
        public int HealthAfter { get; set; }
        public DamageSources DamageSource { get; set; }

        public override string ToString()
        {
            return $"TK took hit: Damage={Damage} HealthAfter={HealthAfter} DamageSource={DamageSource}";
        }
    }
}
