using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class ChallengeMenuInjector : Handler
    {
        public override void Load(IGodhomeWinLossTracker mod, TheMessageBus bus)
        {
            base.Load(mod, bus);
            On.BossDoorChallengeUI.Setup += BossDoorChallengeUI_Setup;
            On.BossChallengeUI.Setup += BossChallengeUI_Setup;
        }

        private void BossDoorChallengeUI_Setup(On.BossDoorChallengeUI.orig_Setup orig, BossDoorChallengeUI self, BossSequenceDoor door)
        {
            orig(self, door);

            if (_mod.globalData.ShowStatsInChallengeMenu)
            {
                // Unidentified pantheons are okay to pass into the query.
                // They will change the sequence to null.
                int? indexq = GodhomeUtils.GetPantheonIndexFromDescriptionKey(door.descriptionKey);

                var attributes = self.descriptionText.text == "GodSeeker+/SelectSegment".Localize()
                    ? GodhomeUtils.PantheonAttributes.IsSegment
                    : GodhomeUtils.PantheonAttributes.None;

                _bus.Put(new PantheonStatsQuery(indexq, attributes, (runs, pb, churns) =>
                {
                    if (runs != null)
                    {
                        self.titleTextSuper.text = runs;
                    }
                    if (pb != null)
                    {
                        self.titleTextMain.text = pb;
                    }
                    if (churns != null)
                    {
                        self.descriptionText.text = churns;
                    }
                }));
            }
        }

        private void BossChallengeUI_Setup(On.BossChallengeUI.orig_Setup orig, BossChallengeUI self, BossStatue bossStatue, string bossNameSheet, string bossNameKey, string descriptionSheet, string descriptionKey)
        {
            orig(self, bossStatue, bossNameSheet, bossNameKey, descriptionSheet, descriptionKey);

            if (_mod.globalData.ShowStatsInChallengeMenu)
            {
                _bus.Put(new HoGStatsQuery(bossNameKey, statsText =>
                {
                    if (statsText != null)
                    {
                        self.descriptionText.text = statsText;
                    }
                }));
            }
        }
    }
}
