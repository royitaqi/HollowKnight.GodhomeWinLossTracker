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
        private static readonly string ModSaveDirectory = Application.persistentDataPath + "/GodhomeWinLossTracker";
        private static readonly string ModBackupDirectory = ModSaveDirectory + "/backup";

        public SaveLoad(IGodhomeWinLossTracker mod)
        {
            _mod = mod;
            Directory.CreateDirectory(ModSaveDirectory);
            Directory.CreateDirectory(ModBackupDirectory);
        }

        public void OnMessage(TheMessageBus bus, Modding.ILogger logger, IMessage message)
        {
            if (message is SaveFolderData)
            {
                SaveFolderData(bus, logger);
                if (_mod.globalData.AutoExport)
                {
                    ExportFolderData(bus, logger);
                }
            }
            else if (message is LoadFolderData)
            {
                LoadFolderData(bus, logger);
                BackupFolderData(bus, logger);
            }
            else if (message is ExportFolderData)
            {
                ExportFolderData(bus, logger);
            }
        }

        private void SaveFolderData(TheMessageBus bus, Modding.ILogger logger)
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
            logger.Log($"{path} saved: {_mod.folderData.RawRecords.Count} records");
#endif
        }

        private void LoadFolderData(TheMessageBus bus, Modding.ILogger logger)
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
                logger.Log($"{path} loaded: {_mod.folderData.RawRecords.Count} records");
#endif
            }
            else
            {
                _mod.folderData = new FolderData();
#if DEBUG
                logger.Log($"{path} doesn't exist. New/empty folder data will be used.");
#endif
            }
        }

        private void BackupFolderData(TheMessageBus bus, Modding.ILogger logger)
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
                logger.Log($"{path} backed up to {backupPath}");
#endif
            }
        }

        private void ExportFolderData(TheMessageBus bus, Modding.ILogger logger)
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
            logger.Log($"{path} exported: {_mod.folderData.RawRecords.Count} lines");
#endif
            bus.Put(new ExportedFolderData { Filename = filename });
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

        private readonly IGodhomeWinLossTracker _mod;
    }
}
