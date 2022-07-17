namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class ExportFolderData : IMessage
    {
        public int Slot { get; set; }
        public override string ToString()
        {
            return $"Export folder data (slot {Slot})";
        }
    }
}
