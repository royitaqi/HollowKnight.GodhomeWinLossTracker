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

        public SaveLoad()
        {
            Directory.CreateDirectory(ModSaveDirectory);
            Directory.CreateDirectory(ModBackupDirectory);
        }

        public void OnSaveFolderData(SaveFolderData msg)
        {
            SaveFolderData();
            if (_mod.globalData.AutoExport)
            {
                ExportFolderData();
            }
        }

        public void OnLoadFolderData(LoadFolderData msg)
        {
            LoadFolderData();
            BackupFolderData();
        }

        public void OnExportFolderData(ExportFolderData msg)
        {
            ExportFolderData();
        }

        private void SaveFolderData()
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
            _logger.LogMod($"{path} saved: {_mod.folderData.RawWinLosses.Count} records");
        }

        private void LoadFolderData()
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
                _logger.LogMod($"{path} loaded: {_mod.folderData.RawWinLosses.Count} records");
            }
            else
            {
                _mod.folderData = new FolderData();
                _logger.LogMod($"{path} doesn't exist. New/empty folder data will be used.");
            }
        }

        private void BackupFolderData()
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
                _logger.LogMod($"{path} backed up to {backupPath}");
            }
        }

        private void ExportFolderData()
        {
            ExportWinLoss();
            ExportTKHit();
            _bus.Put(new ExportedFolderData());
        }

        private void ExportWinLoss()
        {
            string filename = GetExportWinLossSaveFilename();
            // Skip if not a valid profile ID
            if (filename == null)
            {
                return;
            }
            ExportList(filename, _mod.folderData.RawWinLosses);
        }

        private void ExportTKHit()
        {
            string filename = GetExportTKHitSaveFilename();
            // Skip if not a valid profile ID
            if (filename == null)
            {
                return;
            }
            ExportList(filename, _mod.folderData.RawHits);
        }

        private void ExportList<T>(string filename, List<T> list)
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
            _logger.LogMod($"{path} exported: {list.Count} lines");
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
    }
}
