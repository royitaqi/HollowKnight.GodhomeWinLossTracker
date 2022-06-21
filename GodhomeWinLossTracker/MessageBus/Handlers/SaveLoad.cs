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
        private static readonly string LocalDirectory = Application.persistentDataPath + "/GodhomeWinLossTracker";

        public SaveLoad(GodhomeWinLossTracker mod)
        {
            _mod = mod;
            Directory.CreateDirectory(LocalDirectory);
        }

        public void OnMessage(TheMessageBus bus, Modding.Loggable logger, IMessage message)
        {
            if (message is SaveLocalData)
            {
                try
                {
                    SaveLocalData();
                }
                catch (IOException exception)
                {
                    logger.LogError($"Failed to save local data: {exception}");
                }
            }
            else if (message is LoadLocalData)
            {
                try
                {
                    LoadLocalData();
                }
                catch (IOException exception)
                {
                    logger.LogError($"Failed to load local data: {exception}");
                }
            }
        }

        private void SaveLocalData()
        {
            string filename = GetDataSavePath();
            string jsonString = JsonConvert.SerializeObject(_mod.localData);
            File.WriteAllText(filename, jsonString);
        }

        private void LoadLocalData()
        {
            string filename = GetDataSavePath();
            if (File.Exists(filename))
            {
                string jsonString = File.ReadAllText(filename);
                _mod.localData = JsonConvert.DeserializeObject<LocalData>(jsonString);
            }
            else
            {
                _mod.Log($"{filename} doesn't exist. New/empty local data will be used.");
            }
        }

        private string GetDataSavePath()
        {
            int slot = GameManager.instance.profileID;
            return $"{LocalDirectory}/Data.Save{slot}.json";
        }

        private readonly GodhomeWinLossTracker _mod;
    }
}
