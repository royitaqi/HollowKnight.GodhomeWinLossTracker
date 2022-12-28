using GodhomeWinLossTracker;
using GodhomeWinLossTracker.Utils;

namespace UnitTests
{
    [TestClass]
    public class TestSaveLoadUtils
    {
        internal VersionedFolderData GetVersionedFolderData_v_0_7_0_0()
        {
            var data = new FolderData();
            data.RawHits = new List<RawHit>
            {
                // These should be wiped
                new RawHit("test ts", "test seq", "test boss", "test scene", 0, 0, 0, 0, 0, "0", "z", 0, "test state", 0, 0, 0, RecordSources.Test, 0),
                new RawHit("test ts", "test seq", "test boss", "test scene", 0, 0, 0, 0, 0, "1", "z", 0, "test state", 0, 0, 0, RecordSources.Test, 0),
                new RawHit("test ts", "test seq", "test boss", "test scene", 0, 0, 0, 0, 0, "9", "z", 0, "test state", 0, 0, 0, RecordSources.Test, 0),
                // These should remain unchanged
                new RawHit("test ts", "test seq", "test boss", "test scene", 0, 0, 0, 0, 0, "10", "z", 0, "test state", 0, 0, 0, RecordSources.Test, 0),
                new RawHit("test ts", "test seq", "test boss", "test scene", 0, 0, 0, 0, 0, "11", "z", 0, "test state", 0, 0, 0, RecordSources.Test, 0),
                new RawHit("test ts", "test seq", "test boss", "test scene", 0, 0, 0, 0, 0, "19", "z", 0, "test state", 0, 0, 0, RecordSources.Test, 0),
                new RawHit("test ts", "test seq", "test boss", "test scene", 0, 0, 0, 0, 0, "Orb", "Hero Hurter", 0, "test state", 0, 0, 0, RecordSources.Test, 0),
                new RawHit("test ts", "test seq", "test boss", "test scene", 0, 0, 0, 0, 0, "Face Beam", "Radiance Beam", 0, "test state", 0, 0, 0, RecordSources.Test, 0),
            };

            return new VersionedFolderData
            {
                Version = "0.7.0.0",
                FolderData = data,
            };
        }

        internal VersionedFolderData GetVersionedFolderData_v_0_7_1_0()
        {
            var data = new FolderData();
            data.RawHits = new List<RawHit>
            {
                // These should be wiped
                new RawHit("test ts", "test seq", "test boss", "test scene", 0, 0, 0, 0, 0, null, null, 0, "test state", 0, 0, 0, RecordSources.Test, 0),
                new RawHit("test ts", "test seq", "test boss", "test scene", 0, 0, 0, 0, 0, null, null, 0, "test state", 0, 0, 0, RecordSources.Test, 0),
                new RawHit("test ts", "test seq", "test boss", "test scene", 0, 0, 0, 0, 0, null, null, 0, "test state", 0, 0, 0, RecordSources.Test, 0),
                // These should remain unchanged
                new RawHit("test ts", "test seq", "test boss", "test scene", 0, 0, 0, 0, 0, "10", "z", 0, "test state", 0, 0, 0, RecordSources.Test, 0),
                new RawHit("test ts", "test seq", "test boss", "test scene", 0, 0, 0, 0, 0, "11", "z", 0, "test state", 0, 0, 0, RecordSources.Test, 0),
                new RawHit("test ts", "test seq", "test boss", "test scene", 0, 0, 0, 0, 0, "19", "z", 0, "test state", 0, 0, 0, RecordSources.Test, 0),
                new RawHit("test ts", "test seq", "test boss", "test scene", 0, 0, 0, 0, 0, "Orb", "Hero Hurter", 0, "test state", 0, 0, 0, RecordSources.Test, 0),
                new RawHit("test ts", "test seq", "test boss", "test scene", 0, 0, 0, 0, 0, "Face Beam", "Radiance Beam", 0, "test state", 0, 0, 0, RecordSources.Test, 0),
            };

            return new VersionedFolderData
            {
                Version = "0.7.1.0",
                FolderData = data,
            };
        }
        internal VersionedFolderData GetEmptyVersionedFolderData()
        {
            return new VersionedFolderData
            {
                Version = null,
                FolderData = new FolderData(),
            };
        }

        [TestMethod]
        public void TestConvertVersionToInt()
        {
            Assert.AreEqual(1020304, SaveLoadUtils.ConvertVersionToInt("1.2.3.4-abcd"));
            Assert.AreEqual(11223344, SaveLoadUtils.ConvertVersionToInt("11.22.33.44-abcd"));
            Assert.AreEqual(11223344, SaveLoadUtils.ConvertVersionToInt("11.22.33.44"));
        }

        [TestMethod]
        public void TestFolderDataAdaptor_v_0_7_1_0_Adapt()
        {
            var origVfd = GetVersionedFolderData_v_0_7_0_0();
            var expectedVfd = GetVersionedFolderData_v_0_7_1_0();

            var adaptor = new FolderDataAdaptor_v_0_7_1_0();
            adaptor.Adapt(origVfd);

            AssertEqual(expectedVfd, origVfd);
        }

        [TestMethod]
        public void TestSaveLoad_v_0_7_1_0_Self()
        {
            var origVfd = GetVersionedFolderData_v_0_7_1_0();
            var expectedVfd = GetVersionedFolderData_v_0_7_1_0();

            var path = Path.GetTempFileName();

            // Write out data.
            SaveLoadUtils.Save(path, origVfd);

            // Read back in.
            var vfd = SaveLoadUtils.Load(path);

            // Check that data has been correctly written/read.
            AssertEqual(expectedVfd, vfd);
        }

        [TestMethod]
        public void TestSaveLoad_v_0_7_1_0_Adapt()
        {
            var origVfd = GetVersionedFolderData_v_0_7_0_0();
            var expectedVfd = GetVersionedFolderData_v_0_7_1_0();

            var path = Path.GetTempFileName();

            // Write out old folder data in json format.
            new JsonSaver().Save(path, origVfd);

            // Read back in.
            var vfd = SaveLoadUtils.Load(path);

            // Check that data has been correctly written/read and also adapted.
            AssertEqual(expectedVfd, vfd);
        }

        [TestMethod]
        public void TestSaveLoad_NoSaveFile()
        {
            var path = Path.GetTempFileName();

            // Attempt to read. Should fail by returning null.
            var vfd = SaveLoadUtils.Load(path);

            // Check that data has been correctly written/read and also adapted.
            Assert.AreEqual(null, vfd);
        }

        internal string ConvertFolderDataToString(FolderData data)
        {
            StringBuilder sb = new();
            sb.AppendLine("");
            sb.AppendLine("Win/losses:");
            foreach (var winloss in data.RawWinLosses)
            {
                sb.AppendLine("  " + TestUtils.ConvertMessageToString(winloss));
            }
            sb.AppendLine("Hits:");
            foreach (var hit in data.RawHits)
            {
                sb.AppendLine("  " + TestUtils.ConvertMessageToString(hit));
            }
            sb.AppendLine("Phases:");
            foreach (var phase in data.RawPhases)
            {
                sb.AppendLine("  " + TestUtils.ConvertMessageToString(phase));
            }
            return sb.ToString();
        }

        internal void AssertEqual(VersionedFolderData expected, VersionedFolderData actual)
        {
            Assert.AreEqual(expected.Version, actual.Version);
            AssertEqual(expected.FolderData, actual.FolderData);
        }
        internal void AssertEqual(FolderData expected, FolderData actual)
        {
            Assert.AreEqual(
                ConvertFolderDataToString(expected),
                ConvertFolderDataToString(actual)
            );
        }
    }
}