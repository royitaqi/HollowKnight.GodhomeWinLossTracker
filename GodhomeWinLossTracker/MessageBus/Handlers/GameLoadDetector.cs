using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GodhomeWinLossTracker.MessageBus.Messages;
using UnityEngine;
using System.IO;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class GameLoadDetector : IHandler
    {
        public void OnMessage(TheMessageBus bus, Modding.ILogger logger, IMessage message)
        {
            if (message is LoadFolderData)
            {
                _freshlyLoaded = true;
            }
            else if (message is SceneChange)
            {
                // Trigger game loaded event by first scene change
                if (_freshlyLoaded)
                {
                    _freshlyLoaded = false;
                    bus.Put(new GameLoaded());
                }
            }
        }

        private bool _freshlyLoaded = false;
    }
}
