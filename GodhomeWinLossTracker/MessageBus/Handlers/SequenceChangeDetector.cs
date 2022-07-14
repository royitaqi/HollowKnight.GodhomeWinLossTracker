﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class SequenceChangeDetector: Handler
    {
        // Detect HoG sequence by scene GG_Workshop
        public void OnSceneChange(TheMessageBus bus, Modding.ILogger logger, SceneChange msg)
        {
            string currentSceneName = msg.Name;
            DevUtils.Assert(currentSceneName != null, "currentSceneName shouldn't be null");

            if (currentSceneName == "GG_Workshop")
            {
                bus.Put(new SequenceChange { Name = "HoG" });
            }
        }

        public void OnPantheonStatsQuery(TheMessageBus bus, Modding.ILogger logger, PantheonStatsQuery msg)
        {
            // Detect pantheon sequences by stats queries.
            // When the pantheon index is null (i.e. unidentified pantheon), the sequence name becomes null as returned by `GodhomeUtils.GetPantheonSequenceName()`.
            bus.Put(new SequenceChange { Name = GodhomeUtils.GetPantheonSequenceName(msg.PantheonIndex, msg.PantheonAttribute) });
        }
    }
}
