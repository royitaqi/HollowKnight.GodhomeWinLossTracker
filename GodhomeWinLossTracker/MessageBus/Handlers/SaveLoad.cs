using System;
using System.IO;
using GodhomeWinLossTracker.MessageBus.Messages;
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

            var ps = typeof(RawWinLoss).GetProperties();

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
                foreach (var r in _mod.folderData.RawRecords)
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
