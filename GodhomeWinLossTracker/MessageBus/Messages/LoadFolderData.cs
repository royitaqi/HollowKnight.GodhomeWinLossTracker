namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class LoadFolderData : IMessage
    {
        public int Slot { get; set; }
        public override string ToString()
        {
            return $"Load folder data (slot {Slot})";
        }
    }
}
