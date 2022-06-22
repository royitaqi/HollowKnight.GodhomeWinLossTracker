﻿using System;
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
        private static readonly string ModSaveDirectory = Application.persistentDataPath + "/GodhomeWinLossTracker";
        private static readonly string ModBackupDirectory = ModSaveDirectory + "/backup";

        public SaveLoad(GodhomeWinLossTracker mod)
        {
            _mod = mod;
            Directory.CreateDirectory(ModSaveDirectory);
            Directory.CreateDirectory(ModBackupDirectory);
        }

        public void OnMessage(TheMessageBus bus, Modding.Loggable logger, IMessage message)
        {
            if (message is SaveFolderData)
            {
                SaveFolderData(logger);
                if (_mod.globalData.AutoExport)
                {
                    ExportFolderData(logger);
                }
            }
            else if (message is LoadFolderData)
            {
                LoadFolderData(logger);
                BackupFolderData(logger);
            }
            else if (message is ExportFolderData)
            {
                ExportFolderData(logger);
            }
        }

        private void SaveFolderData(Modding.Loggable logger)
        {
            string filename = GetDataSaveFilename();
            // Skip if not a valid profile ID
            if (filename == null)
            {
                return;
            }
            string path = Path.Combine(ModSaveDirectory, filename);

            string jsonString = JsonConvert.SerializeObject(_mod.folderData, Formatting.Indented);

            File.WriteAllText(path, jsonString);
#if DEBUG
            _mod.Log($"{path} saved: {_mod.folderData.RawRecords.Count} records");
#endif
        }

        private void LoadFolderData(Modding.Loggable logger)
        {
            string filename = GetDataSaveFilename();
            // Skip if not a valid profile ID
            if (filename == null)
            {
                return;
            }
            string path = Path.Combine(ModSaveDirectory, filename);

            if (File.Exists(path))
            {
                string jsonString = File.ReadAllText(path);
                _mod.folderData = JsonConvert.DeserializeObject<FolderData>(jsonString);
#if DEBUG
                _mod.Log($"{path} loaded: {_mod.folderData.RawRecords.Count} records");
#endif
            }
            else
            {
                _mod.folderData = new FolderData();
#if DEBUG
                _mod.Log($"{path} doesn't exist. New/empty folder data will be used.");
#endif
            }
        }

        private void BackupFolderData(Modding.Loggable logger)
        {
            string filename = GetDataSaveFilename();
            // Skip if not a valid profile ID
            if (filename == null)
            {
                return;
            }
            string path = Path.Combine(ModSaveDirectory, filename);

            string backupSuffix = DateTime.Now.ToString("yyyyMMdd.HHmmss.fff") + ".json";
            string backupPath = Path.Combine(ModBackupDirectory, $"{filename}.{backupSuffix}");

            if (File.Exists(path))
            {
                File.Copy(path, backupPath);
#if DEBUG
                _mod.Log($"{path} backed up to {backupPath}");
#endif
            }
        }

        private void ExportFolderData(Modding.Loggable logger)
        {
            string filename = GetExportSaveFilename();
            // Skip if not a valid profile ID
            if (filename == null)
            {
                return;
            }
            string path = Path.Combine(ModSaveDirectory, filename);

            using (var sw = new System.IO.StreamWriter(path))
            {
                sw.WriteLine($"Date/Time\tSequence\tBoss\tScene\tWins\tLosses\tFight Length (ms)\tSource");
                foreach (var r in _mod.folderData.RawRecords)
                {
                    sw.WriteLine($"{r.Timestamp}\t{r.SequenceName}\t{r.BossName}\t{r.SceneName}\t{r.Wins}\t{r.Losses}\t{r.FightLengthMs}\t{r.Source}");
                }
            }
#if DEBUG
            _mod.Log($"{path} exported: {_mod.folderData.RawRecords.Count} lines");
#endif
            _mod.messageBus.Put(new ExportedFolderData { Filename = filename });
        }

        private string GetDataSaveFilename()
        {
            int slot = _mod.localData.ProfileID;
            if (slot == 0)
            {
                return null;
            }
            return $"Data.Save{slot}.json";
        }

        private string GetExportSaveFilename()
        {
            int slot = _mod.localData.ProfileID;
            if (slot == 0)
            {
                return null;
            }
            return $"Export.Save{slot}.txt";
        }

        private readonly GodhomeWinLossTracker _mod;
    }
}
