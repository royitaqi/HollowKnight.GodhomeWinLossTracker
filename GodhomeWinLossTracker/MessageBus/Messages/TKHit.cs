using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    [ModLogLevel(Modding.LogLevel.Debug)]
    public class TKHit: IMessage
    {
        public int Damage { get; set; }
        public int HealthBefore => HealthAfter + Damage;
        public int HealthAfter { get; set; }
        public string DamageSource { get; set; }
        public string DamageSourceDetail { get; set; }

        public override string ToString()
        {
            return $"TK took hit: Damage={Damage} HealthAfter={HealthAfter} DamageSource={DamageSource} DamageSourceDetail={DamageSourceDetail}";
        }
    }
}
