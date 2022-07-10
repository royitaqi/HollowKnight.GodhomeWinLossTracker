using System;
using System.Collections.Generic;
using System.IO;
using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class SaveLoad : Handler
    {
        private static readonly string ModSaveDirectory = Application.persistentDataPath + "/GodhomeWinLossTracker";
        private static readonly string ModBackupDirectory = ModSaveDirectory + "/backup";

        public SaveLoad(IGodhomeWinLossTracker mod)
        {
            _mod = mod;
            Directory.CreateDirectory(ModSaveDirectory);
            Directory.CreateDirectory(ModBackupDirectory);
        }

        public void OnSaveFolderData(TheMessageBus bus, Modding.ILogger logger, SaveFolderData msg)
        {
            SaveFolderData(bus, logger);
            if (_mod.globalData.AutoExport)
            {
                ExportFolderData(bus, logger);
            }
        }

        public void OnLoadFolderData(TheMessageBus bus, Modding.ILogger logger, LoadFolderData msg)
        {
            LoadFolderData(bus, logger);
            BackupFolderData(bus, logger);
        }

        public void OnExportFolderData(TheMessageBus bus, Modding.ILogger logger, ExportFolderData msg)
        {
            ExportFolderData(bus, logger);
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
            logger.LogMod($"{path} saved: {_mod.folderData.RawWinLosses.Count} records");
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
                logger.LogMod($"{path} loaded: {_mod.folderData.RawWinLosses.Count} records");
            }
            else
            {
                _mod.folderData = new FolderData();
                logger.LogMod($"{path} doesn't exist. New/empty folder data will be used.");
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
                logger.LogMod($"{path} backed up to {backupPath}");
            }
        }

        private void ExportFolderData(TheMessageBus bus, Modding.ILogger logger)
        {
            ExportWinLoss(bus, logger);
            ExportTKHit(bus, logger);
            bus.Put(new ExportedFolderData());
        }

        private void ExportWinLoss(TheMessageBus bus, Modding.ILogger logger)
        {
            string filename = GetExportWinLossSaveFilename();
            // Skip if not a valid profile ID
            if (filename == null)
            {
                return;
            }
            ExportList(bus, logger, filename, _mod.folderData.RawWinLosses);
        }

        private void ExportTKHit(TheMessageBus bus, Modding.ILogger logger)
        {
            string filename = GetExportTKHitSaveFilename();
            // Skip if not a valid profile ID
            if (filename == null)
            {
                return;
            }
            ExportList(bus, logger, filename, _mod.folderData.RawHits);
        }

        private void ExportList<T>(TheMessageBus bus, Modding.ILogger logger, string filename, List<T> list)
        {
            string path = Path.Combine(ModSaveDirectory, filename);

            var ps = typeof(T).GetProperties();

            using (var sw = new System.IO.StreamWriter(path))
            {
                // Write header
                bool first = true;
                foreach (var p in ps)
                {
                    if (!first) sw.Write('\t');
                    else first = false;

                    sw.Write(p.Name);
                }
                sw.WriteLine();

                // Write values
                foreach (var r in list)
                {
                    first = true;
                    foreach (var p in ps)
                    {
                        if (!first) sw.Write('\t');
                        else first = false;

                        sw.Write(p.GetValue(r));
                    }
                    sw.WriteLine();
                }
            }
            logger.LogMod($"{path} exported: {list.Count} lines");
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

        private string GetExportWinLossSaveFilename()
        {
            int slot = _mod.localData.ProfileID;
            if (slot == 0)
            {
                return null;
            }
            return $"Export.WinLoss.Save{slot}.txt";
        }

        private string GetExportTKHitSaveFilename()
        {
            int slot = _mod.localData.ProfileID;
            if (slot == 0)
            {
                return null;
            }
            return $"Export.Hit.Save{slot}.txt";
        }

        private readonly IGodhomeWinLossTracker _mod;
    }
}
