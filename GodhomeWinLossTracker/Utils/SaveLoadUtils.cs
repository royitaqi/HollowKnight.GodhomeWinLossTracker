using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using GodhomeWinLossTracker.MessageBus.Messages;
using Newtonsoft.Json;
using Vasi;

namespace GodhomeWinLossTracker.Utils
{
    internal static class SaveLoadUtils
    {
        /**
         * Returns null when no loader can load (e.g. when there is no save file to load).
         */
        public static VersionedFolderData Load(string path)
        {
            VersionedFolderData vfd = FolderDataLoader.Load(path);
            if (vfd == null)
            {
                return null;
            }

            FolderDataAdaptor.Adapt(vfd);
            return vfd;
        }

        public static void Save(string path, VersionedFolderData vfd)
        {
            FolderDataSaver.Save(path, vfd);
        }

        internal static int ConvertVersionToInt(string version)
        {
            Regex pattern = new Regex(@"(?<A>\d+).(?<B>\d+).(?<C>\d+).(?<D>\d+)(-(\w+))?");
            Match match = pattern.Match(version);
            return
                int.Parse(match.Groups["A"].Value) * 1000000
                + int.Parse(match.Groups["B"].Value) * 10000
                + int.Parse(match.Groups["C"].Value) * 100
                + int.Parse(match.Groups["D"].Value);
        }
    }

    /// <summary>
    /// Folder Data Loaders
    /// </summary>
    
    internal interface IFolderDataLoader
    {
        /**
         * Should return whether or not necessary files exist.
         */
        public bool CanLoad(string path);

        /**
         * Should never return null.
         */
        public VersionedFolderData Load(string path);
    }

    internal static class FolderDataLoader
    {
        /**
         * Returns null when no loader can load (e.g. when there is no save file to load).
         */
        public static VersionedFolderData Load(string path)
        {
            return Loaders
                .FirstOrDefault(loader => loader.CanLoad(path))
                ?.Load(path);
        }

        private static readonly IFolderDataLoader[] Loaders = new IFolderDataLoader[]
        {
            // IMPORTANT: Loaders should be listed in the order of loading attempts.
            new ZipLoader(),
            new JsonLoader(),
        };
    }

    /**
     * Loads folder data from a .json file.
     */
    internal class JsonLoader: IFolderDataLoader
    {
        public bool CanLoad(string path)
        {
            return File.Exists(path + ".json");
        }

        public VersionedFolderData Load(string path)
        {
            string jsonString = File.ReadAllText(path + ".json");
            return new VersionedFolderData
            {
                // The last version that writes a .json file is v0.7.0.0.
                Version = "0.7.0.0",
                FolderData = JsonConvert.DeserializeObject<FolderData>(jsonString),
            };
        }
    }

    /**
     * Loads folder data from a .zip file.
     */
    internal class ZipLoader : IFolderDataLoader
    {
        public bool CanLoad(string path)
        {
            return File.Exists(path + ".zip");
        }

        public VersionedFolderData Load(string path)
        {
            string version = null;
            FolderData data = null;
            using (var zip = ZipFile.Open(path + ".zip", ZipArchiveMode.Read))
            {
                using (var stream = zip.GetEntry("version.txt").Open())
                using (var streamReader = new StreamReader(stream))
                {
                    // Trim to remove any whitespace and EOL characters.
                    version = streamReader.ReadToEnd().Trim();
                }

                using (var stream = zip.GetEntry("data.txt").Open())
                using (var streamReader = new StreamReader(stream))
                {
                    string json = streamReader.ReadToEnd();
                    data = JsonConvert.DeserializeObject<FolderData>(json);
                }
            }

            return new VersionedFolderData { Version = version, FolderData = data };
        }
    }

    /// <summary>
    /// Folder Data Adaptors
    /// </summary>

    internal interface IFolderDataAdaptor
    {
        public string AdaptToVersion { get; }

        public void Adapt(VersionedFolderData vfd);
    }

    internal static class FolderDataAdaptor
    {
        public static void Adapt(VersionedFolderData vfd)
        {
            foreach (var adaptor in Adaptors)
            {
                // Only run adaptor if data version is smaller than adaptor's version
                if (SaveLoadUtils.ConvertVersionToInt(vfd.Version) < SaveLoadUtils.ConvertVersionToInt(adaptor.AdaptToVersion))
                {
                    adaptor.Adapt(vfd);
                }
            }
        }

        private static readonly IFolderDataAdaptor[] Adaptors = new IFolderDataAdaptor[]
        {
            // IMPORTANT: Adaptors should be listed in ascending order of their version numbers.
            new FolderDataAdaptor_v_0_7_1_0(),
        };
    }

    /**
     * # What changed in this version
     * 
     * In v0.7.1.0, in **hits* data, the type and meaning of "DamageSource" has changed:
     * - Before v0.7.1.0: int, 0-2
     * - At v0.7.1.0: string, a human-understandable representation of the damage source, e.g. "Orb" or "Face Beam"
     * 
     * 
     * # How does the adaptor migrate this change
     * 
     * - The numbers can be parsed into JSON as strings, so 0 become "0".
     * - The adaptor will look for single digit numbers.
     * - Once found, the adaptor will wipe both damage source and damage source detail.
     */
    internal class FolderDataAdaptor_v_0_7_1_0 : IFolderDataAdaptor
    {
        public string AdaptToVersion => "0.7.1.0";

        public void Adapt(VersionedFolderData vfd)
        {
            DevUtils.Assert(
                SaveLoadUtils.ConvertVersionToInt(vfd.Version) < SaveLoadUtils.ConvertVersionToInt(AdaptToVersion),
                $"Should adaptor from a lower version ({vfd.Version}) to a higher version ({AdaptToVersion})"
            );

            foreach (var hit in vfd.FolderData.RawHits)
            {
                if (hit.DamageSource != null && hit.DamageSource.Length == 1 && hit.DamageSource[0] >= '0' && hit.DamageSource[0] <= '9')
                {
                    // Use Reflection to work around the private setter
                    PropertyInfo ds = typeof(RawHit).GetProperty("DamageSource");
                    ds.SetValue(hit, null);
                    PropertyInfo dsd = typeof(RawHit).GetProperty("DamageSourceDetail");
                    dsd.SetValue(hit, null);
                }
            }

            vfd.Version = AdaptToVersion;
        }
    }

    /// <summary>
    /// Folder Data Savers
    /// </summary>

    internal interface IFolderDataSaver
    {
        public void Save(string path, VersionedFolderData vfd);
    }

    internal static class FolderDataSaver
    {
        public static void Save(string path, VersionedFolderData vfd)
        {
            Saver.Save(path, vfd);
        }

        private static readonly IFolderDataSaver Saver = new ZipSaver();
    }

    internal class ZipSaver : IFolderDataSaver
    {
        public void Save(string path, VersionedFolderData vfd)
        {
            path += ".zip";

            // Have to delete the file, because ZipFile.Open() doesn't support a CreateOrNew mode.
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (var zip = ZipFile.Open(path, ZipArchiveMode.Create))
            {
                using (var stream = zip.CreateEntry("version.txt").Open())
                using (var streamWriter = new StreamWriter(stream))
                {
                    streamWriter.WriteLine(vfd.Version);
                }

                using (var stream = zip.CreateEntry("data.txt").Open())
                using (var streamWriter = new StreamWriter(stream))
                {
                    string json = JsonConvert.SerializeObject(vfd.FolderData, Formatting.Indented);
                    streamWriter.WriteLine(json);
                }
            }
        }
    }

    internal class JsonSaver : IFolderDataSaver
    {
        public void Save(string path, VersionedFolderData vfd)
        {
            path += ".json";
            string json = JsonConvert.SerializeObject(vfd.FolderData, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}
