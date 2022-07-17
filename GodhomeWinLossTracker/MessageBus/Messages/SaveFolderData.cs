namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class SaveFolderData : IMessage
    {
        public int Slot { get; set; }
        public override string ToString()
        {
            return $"Save folder data (slot {Slot})";
        }
    }
}
