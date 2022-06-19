using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GodhomeWinLossTracker.MessageBus.Messages;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class SequenceChangeDetector: IHandler
    {
        public void OnMessage(TheMessageBus bus, Modding.Loggable logger, IMessage msg)
        {
            if (msg is SceneChange)
            {
                string sceneName = (msg as SceneChange).Name;
                Debug.Assert(sceneName != null);

                // Update the bus with latest recognized sequence
                if (sceneName == "GG_Workshop")
                {
                    bus.Put(new SequenceChange { Name = "HoG" });
                }
                else if (sceneName == "GG_Boss_Door_Entrance")
                {
                    bus.Put(new SequenceChange { Name = "pantheons" });
                }
            }
        }
    }
}
