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
                string currentSceneName = (msg as SceneChange).Name;
                Debug.Assert(currentSceneName != null);

                if (GodhomeUtils.IsBossScene(currentSceneName))
                {
                    string sequenceName = GodhomeUtils.GetSequenceName(_previousSceneName, currentSceneName);
                    if (sequenceName != null)
                    {
                        bus.Put(new SequenceChange { Name = sequenceName });
                    }
                }

                // Record the current scene name, so that it can be used in the next round as the previous scene name.
                _previousSceneName = currentSceneName;
            }
        }

        private string _previousSceneName;
    }
}
