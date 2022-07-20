namespace UnitTests
{
    [TestClass]
    public class TestDataCompatibility
    {
        [TestMethod]
        public void Test_v0_2_1_0()
        {
            string json = @"
            {
                ""RawRecords"": [
                    {
                        ""Timestamp"": ""2022-06-16 10:00:00"",
                        ""SequenceName"": ""P5"",
                        ""BossName"": ""Vengefly King"",
                        ""SceneName"": ""GG_Vengefly_V"",
                        ""Wins"": 1,
                        ""Losses"": 0,
                        ""FightLengthMs"": 138698,
                        ""Source"": 0
                    }
                ]
            }
            ";
            string ret = Run_v0_2_1_0(json);

            // Compare counts
            Assert.AreEqual(1, Get<FolderData_v0_2_1_0>(json).RawRecords.Count);

            // Compare two json
            json = Run<FolderData_v0_2_1_0>(json);
            ret = Run<FolderData_v0_2_1_0>(ret);
            Assert.AreEqual(json, ret);
        }

        [TestMethod]
        public void Test_v0_2_4_0()
        {
            string json = @"
            {
                ""RawRecords"": [
                    {
                        ""Timestamp"": ""2022-06-16 10:00:00"",
                        ""SequenceName"": ""P5"",
                        ""BossName"": ""Vengefly King"",
                        ""SceneName"": ""GG_Vengefly_V"",
                        ""Wins"": 1,
                        ""Losses"": 0,
                        ""FightLengthMs"": 138698,
                        ""Source"": 0
                    }
                ]
            }
            ";
            string ret = Run_v0_2_4_0(json);

            // Compare counts
            Assert.AreEqual(1, Get<FolderData_v0_2_4_0>(json).RawRecords.Count);

            // Compare two json
            json = Run<FolderData_v0_2_4_0>(json);
            ret = Run<FolderData_v0_2_4_0>(ret);
            Assert.AreEqual(json, ret);
        }

        [TestMethod]
        public void Test_v0_3_0_2()
        {
            string json = @"
            {
                ""RawRecords"": [
                    {
                        ""Timestamp"": ""2022 - 06 - 16 10:00:00"",
                        ""SequenceName"": ""P5"",
                        ""BossName"": ""Vengefly King"",
                        ""SceneName"": ""GG_Vengefly_V"",
                        ""Wins"": 1,
                        ""Losses"": 0,
                        ""Heals"": 0,
                        ""HealAmount"": 0,
                        ""Hits"": 0,
                        ""HitAmount"": 0,
                        ""FightLengthMs"": 138698,
                        ""Source"": 0
                    }
                ]
            }
            ";
            string ret = Run_v0_3_0_2(json);

            // Compare counts
            Assert.AreEqual(1, Get<FolderData_v0_3_0_2>(json).RawRecords.Count);

            // Compare two json
            json = Run<FolderData_v0_3_0_2>(json);
            ret = Run<FolderData_v0_3_0_2>(ret);
            Assert.AreEqual(json, ret);
        }

        [TestMethod]
        public void Test_v0_4_3_0()
        {
            string json = @"
            {
                ""RawRecords"": [
                    {
                        ""Timestamp"": ""2022 - 06 - 16 10:00:00"",
                        ""SequenceName"": ""P5"",
                        ""BossName"": ""Vengefly King"",
                        ""SceneName"": ""GG_Vengefly_V"",
                        ""Wins"": 1,
                        ""Losses"": 0,
                        ""Heals"": 0,
                        ""HealAmount"": 0,
                        ""Hits"": 0,
                        ""HitAmount"": 0,
                        ""BossHP"": 0.0,
                        ""FightLengthMs"": 138698,
                        ""RecordSource"": 0
                    }
                ],
                ""RawHits"": [
                    {
                        ""Timestamp"": ""2022 - 07 - 17 21:46:25"",
                        ""SequenceName"": ""P5"",
                        ""BossName"": ""Markoth"",
                        ""SceneName"": ""GG_Ghost_Markoth_V"",
                        ""TKStatus"": 257,
                        ""TKHealthBefore"": 1,
                        ""DamageAmount"": 1,
                        ""DamageSource"": 0,
                        ""BossHP"": 0.41230768,
                        ""BossState"": ""Hover"",
                        ""FightLengthMs"": 114719,
                        ""RecordSource"": 1
                    }
                ]
            }
            ";
            string ret = Run_v0_4_3_0(json);

            // Compare counts
            Assert.AreEqual(2, Get<FolderData_v0_4_3_0>(json).RawWinLosses.Count + Get<FolderData_v0_4_3_0>(json).RawHits.Count);

            // Compare two json
            json = Run<FolderData_v0_4_3_0>(json);
            ret = Run<FolderData_v0_4_3_0>(ret);
            Assert.AreEqual(json, ret);
        }

        [TestMethod]
        public void Test_v0_4_11_0()
        {
            string json = @"
            {
                ""RawRecords"": [
                    {
                        ""Timestamp"": ""2022 - 06 - 16 10:00:00"",
                        ""SequenceName"": ""P5"",
                        ""BossName"": ""Vengefly King"",
                        ""SceneName"": ""GG_Vengefly_V"",
                        ""Wins"": 1,
                        ""Losses"": 0,
                        ""Heals"": 0,
                        ""HealAmount"": 0,
                        ""Hits"": 0,
                        ""HitAmount"": 0,
                        ""BossHP"": 0.0,
                        ""FightLengthMs"": 138698,
                        ""RecordSource"": 0
                    }
                ],
                ""RawHits"": [
                    {
                        ""Timestamp"": ""2022-07-17 21:45:38"",
                        ""SequenceName"": ""P5"",
                        ""BossName"": ""Markoth"",
                        ""SceneName"": ""GG_Ghost_Markoth_V"",
                        ""TKStatus"": 33554689,
                        ""TKPosX"": 24,
                        ""TKPosY"": 14,
                        ""TKHealthBefore"": 4,
                        ""DamageAmount"": 1,
                        ""DamageSource"": 0,
                        ""BossHP"": 0.510769248,
                        ""BossState"": ""Hover"",
                        ""BossPosX"": 16,
                        ""BossPosY"": 20,
                        ""FightLengthMs"": 67913,
                        ""RecordSource"": 1
                    }
                ]
            }
            ";
            string ret = Run_v0_4_11_0(json);

            // Compare counts
            Assert.AreEqual(2, Get<FolderData_v0_4_11_0>(json).RawWinLosses.Count + Get<FolderData_v0_4_11_0>(json).RawHits.Count);

            // Compare two json
            json = Run<FolderData_v0_4_11_0>(json);
            ret = Run<FolderData_v0_4_11_0>(ret);
            Assert.AreEqual(json, ret);
        }

        private string Run_v0_2_1_0(string json)
        {
            string mid = Run<FolderData_v0_2_1_0>(json);
            return Run_v0_2_4_0(mid);
        }

        private string Run_v0_2_4_0(string json)
        {
            string mid = Run<FolderData_v0_2_4_0>(json);
            return Run_v0_3_0_2(mid);
        }

        private string Run_v0_3_0_2(string json)
        {
            string mid = Run<FolderData_v0_3_0_2>(json);
            return Run_v0_4_3_0(mid);
        }

        private string Run_v0_4_3_0(string json)
        {
            string mid = Run<FolderData_v0_4_3_0>(json);
            return Run_v0_4_11_0(mid);
        }

        private string Run_v0_4_11_0(string json)
        {
            return Run<FolderData_v0_4_11_0>(json);
        }

        private string Run<S>(string json)
        {
            var folderData = JsonConvert.DeserializeObject<S>(json);
            return JsonConvert.SerializeObject(folderData, Formatting.Indented);
        }

        private S Get<S>(string json)
        {
            return JsonConvert.DeserializeObject<S>(json);
        }
    }
}

/*
# Manual test log at v0.4.11.0:


Current save v0.4.11.0
- ?? records
Versions of code to test:
- v0.2.1.0
- v0.2.4.0
- v0.3.0.2
- v0.4.3.0
- v0.4.11.0


Downpatch to v0.2.1.0
Load
[x] Loaded empty data. Expected, because no profile ID in local saves.
Save
Copy save v0.4.11.0 into game save
Load
[x] Loaded 1204 records.
Check challenge UI for Lost Kin.
[x] Has stats
Save
[x] Saved 1204 records


Uppatch to v0.2.4.0
Load
[x] Loaded 1204 records.
Check challenge UI for Lost Kin.
[x] Has stats
Save
[x] Saved 1204 records


Uppatch to v0.3.0.2
Load
[x] Loaded 1204 records.
Check challenge UI for Lost Kin.
[x] Has stats
Save
[x] Saved 1204 records


Uppatch to v0.4.3.0
Load
[x] Loaded 1204 records.
Check challenge UI for Lost Kin.
[x] Has stats
Save
[x] Saved 1204 records for win/loss
[x] Saved 0 records for hits
Copy Data.Save1.v0.4.3.0.After.Add.2Hits.json into game save
Load
[x] Loaded 1204 records.
Check challenge UI for Lost Kin.
[x] Has stats
Save
[x] Saved 1204 records for win/loss
[x] Saved 2 records for hits
Compare hits records
[x] They represent the same info


Uppatch to v0.4.11.0
Load
[x] Loaded 1204 records.
Check challenge UI for Lost Kin.
[x] Has stats
Save
[x] Saved 1204 records for win/loss
[x] Saved 2 records for hits
Copy Data.Save1.v0.4.11.0.After.Add.1Hit.json into game save
Load
[x] Loaded 1204 records.
Check challenge UI for Lost Kin.
[x] Has stats
Save
[x] Saved 1204 records for win/loss
[x] Saved 3 records for hits
Compare hits records
[x] They represent the same info
*/