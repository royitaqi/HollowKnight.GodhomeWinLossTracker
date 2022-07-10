using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;
using Modding;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class ChallengeUIInjector : Handler
    {
        public override void Load(IGodhomeWinLossTracker mod, TheMessageBus bus)
        {
            _mod = mod;
            _bus = bus;

            On.BossDoorChallengeUI.Setup += BossDoorChallengeUI_Setup;
            On.BossChallengeUI.Setup += BossChallengeUI_Setup;
        }

        private void BossDoorChallengeUI_Setup(On.BossDoorChallengeUI.orig_Setup orig, BossDoorChallengeUI self, BossSequenceDoor door)
        {
            orig(self, door);

            if (_mod.globalData.ShowStatsInChallengeMenu)
            {
                int? indexq = GodhomeUtils.GetPantheonIndexFromDescriptionKey(door.descriptionKey);
                if (indexq == null)
                {
                    // Unknown pantheon. No change to challenge menu.
                    return;
                }
                int index = (int)indexq;

                _bus.Put(new PantheonStatsQuery(index, (runs, pb, churns) =>
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

        private IGodhomeWinLossTracker _mod;
        private TheMessageBus _bus;
    }
}
