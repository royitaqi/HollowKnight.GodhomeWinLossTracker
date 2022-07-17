namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class BossHpPos : IMessage
    {
        public int MaxHP { get; set; }
        public int HP { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public override string ToString()
        {
            return $"Boss HP={HP}/{MaxHP} Pos=({X}, {Y})";
        }
    }
}
