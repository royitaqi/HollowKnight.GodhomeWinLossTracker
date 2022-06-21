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
    internal class SaveLoad : IHandler
    {
        private static readonly string LocalDirectory = Application.persistentDataPath + "/GodhomeWinLossTracker/";

        public SaveLoad(GodhomeWinLossTracker mod)
        {
            _mod = mod;
            Directory.CreateDirectory(LocalDirectory);
        }

        public void OnMessage(TheMessageBus bus, Modding.Loggable logger, IMessage message)
        {
            if (message is SaveLocalData)
            {
                File.WriteAllText(LocalDirectory + "/tmp.txt", "test");
            }
            else if (message is LoadLocalData)
            {
                try
                {
                    bus.Put(new BusEvent { Event = File.ReadAllText(LocalDirectory + "/tmp.txt") });
                }
                catch (IOException)
                {
                    bus.Put(new BusEvent { Event = $"Couldn't read {LocalDirectory}/tmp.txt" });
                }

            }
        }

        private readonly GodhomeWinLossTracker _mod;
    }
}
