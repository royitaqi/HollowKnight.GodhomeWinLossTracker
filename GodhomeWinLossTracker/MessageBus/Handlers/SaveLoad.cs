using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;
using Modding;
using Newtonsoft.Json;
using UnityEngine;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class SaveLoad : Handler
    {
        public static readonly string ModSaveDirectory = Application.persistentDataPath + "/GodhomeWinLossTracker";

        public override void Load(IGodhomeWinLossTracker mod, TheMessageBus bus, Modding.ILogger logger)
        {
            base.Load(mod, bus, logger);
            ModHooks.SavegameLoadHook += ModHooks_LoadHook;
            ModHooks.SavegameSaveHook += ModHooks_SaveHook;
            Directory.CreateDirectory(ModSaveDirectory);
        }

        public override void Unload()
        {
            // Don't unload
        }

        private void ModHooks_SaveHook(int obj)
        {
            _lastSlot = obj;
            _bus.Put(new SaveFolderData { Slot = obj });
        }

        private void ModHooks_LoadHook(int obj)
        {
            _lastSlot = obj;
            _bus.Put(new LoadFolderData { Slot = obj });
        }

        public void OnBossChange(BossChange msg)
        {
            // Save when game is advancing to the next boss (in Pantheons).
            if (_lastBoss.IsBoss() && msg.IsBoss())
            {
                DevUtils.Assert(_lastSlot.HasValue, "Should have loaded/saved before entering a boss scene.");
                _bus.Put(new SaveFolderData { Slot = _lastSlot.Value });
            }
            _lastBoss = msg;
        }
        private int? _lastSlot = null;
        private BossChange _lastBoss = new BossChange();

        public void OnSaveFolderData(SaveFolderData msg)
        {
            // Skip saving when data hasn't changed and the save is not forced
            if (GetDataHash() == _loadedDataHash && !msg.Forced)
            {
                _logger.LogMod($"Skipping save because current data hash equals to loaded data hash ({_loadedDataHash})");
                return;
            }

            WriteAndMaybeWait(() =>
            {
                SaveFolderData(msg.Slot);
                if (_mod.globalData.AutoExport)
                {
                    ExportFolderData(msg.Slot);
                }
            });

            _loadedDataHash = GetDataHash();
            _logger.LogMod($"Updating data hash to {_loadedDataHash}");
        }

        public void OnLoadFolderData(LoadFolderData msg)
        {
            LoadFolderData(msg.Slot);

            _loadedDataHash = GetDataHash();
            _logger.LogMod($"Updating data hash to {_loadedDataHash}");
        }

        public void OnExportFolderData(ExportFolderData msg)
        {
            // Skip saving when data hasn't changed
            if (GetDataHash() == _loadedDataHash)
            {
                _logger.LogMod($"Skipping export because current data hash equals to loaded data hash ({_loadedDataHash})");
                return;
            }

            WriteAndMaybeWait(() =>
            {
                ExportFolderData(msg.Slot);
            });
        }

        private void WriteAndMaybeWait(Action write)
        {
            // Wrap the write operation so that async exceptions that are thrown in the Task can be logged to mod log
            Task writeJson = new Task(() =>
            {
                try
                {
                    write();
                }
                catch (Exception ex)
                {
                    _logger.LogModWarn($"Async exception was thrown from SaveLoad: {ex.ToString()}");
                }
            });
            writeJson.Start();

            // If writes should be synchronize, wait for writes.
            if (!_mod.globalData.AsyncWrites)
            {
                writeJson.Wait();
            }
        }

        private void SaveFolderData(int slot)
        {
            string filename = GetDataSaveFilename(slot);
            // Skip if not a valid profile ID
            if (filename == null)
            {
                return;
            }
            string path = Path.Combine(ModSaveDirectory, filename);

            SaveLoadUtils.Save(path, new VersionedFolderData
            {
                Version = _mod.GetVersion(),
                FolderData = _mod.folderData,
            });

            _logger.LogMod($"{path} saved: {_mod.folderData.RawWinLosses.Count} wins/losses, {_mod.folderData.RawHits.Count} hits, {_mod.folderData.RawPhases.Count} phases");
        }

        private void LoadFolderData(int slot)
        {
            string filename = GetDataSaveFilename(slot);
            // Skip if not a valid profile ID
            if (filename == null)
            {
                return;
            }
            string path = Path.Combine(ModSaveDirectory, filename);

            var vfd = SaveLoadUtils.Load(path);
            if (vfd != null)
            {
                _mod.folderData = vfd.FolderData;
                _logger.LogMod($"{path} loaded: {_mod.folderData.RawWinLosses.Count} wins/losses, {_mod.folderData.RawHits.Count} hits, {_mod.folderData.RawPhases.Count} phases");
            }
            else
            {
                _mod.folderData = new FolderData();
                _logger.LogMod($"{path} doesn't exist. New/empty folder data will be used.");
            }
        }

        private void ExportFolderData(int slot)
        {
            ExportWinLoss(slot);
            ExportHit(slot);
            ExportPhases(slot);
            _bus.Put(new ExportedFolderData());
        }

        private void ExportWinLoss(int slot)
        {
            string filename = GetExportWinLossSaveFilename(slot);
            // Skip if not a valid profile ID
            if (filename == null)
            {
                return;
            }
            ExportList(filename, _mod.folderData.RawWinLosses);
        }

        private void ExportHit(int slot)
        {
            string filename = GetExportHitSaveFilename(slot);
            // Skip if not a valid profile ID
            if (filename == null)
            {
                return;
            }
            ExportList(filename, _mod.folderData.RawHits);
        }

        private void ExportPhases(int slot)
        {
            string filename = GetExportPhaseSaveFilename(slot);
            // Skip if not a valid profile ID
            if (filename == null)
            {
                return;
            }
            ExportList(filename, _mod.folderData.RawPhases);
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

        private string GetDataSaveFilename(int slot)
        {
            if (slot == 0)
            {
                return null;
            }
            return $"Data.Save{slot}";
        }

        private string GetExportWinLossSaveFilename(int slot)
        {
            if (slot == 0)
            {
                return null;
            }
            return $"Export.WinLoss.Save{slot}.txt";
        }

        private string GetExportHitSaveFilename(int slot)
        {
            if (slot == 0)
            {
                return null;
            }
            return $"Export.Hit.Save{slot}.txt";
        }

        private string GetExportPhaseSaveFilename(int slot)
        {
            if (slot == 0)
            {
                return null;
            }
            return $"Export.Phase.Save{slot}.txt";
        }

        private int GetDataHash()
        {
            return _mod.folderData.RawWinLosses.Count + _mod.folderData.RawHits.Count + _mod.folderData.RawPhases.Count;
        }

        private int _loadedDataHash;
    }
}
