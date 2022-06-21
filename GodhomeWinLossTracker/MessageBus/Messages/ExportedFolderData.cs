using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class ExportedFolderData : IMessage
    {
        public string Filename { get; set; }

        public override string ToString()
        {
            return $"Exported to {Filename}";
        }
    }
}
