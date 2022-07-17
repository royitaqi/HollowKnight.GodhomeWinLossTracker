using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class SequenceChangeDetector: Handler
    {
        // Detect HoG sequence by scene GG_Workshop
        public void OnSceneChange(SceneChange msg)
        {
            string currentSceneName = msg.Name;
            DevUtils.Assert(currentSceneName != null, "currentSceneName shouldn't be null");

            if (currentSceneName == "GG_Workshop")
            {
                _bus.Put(new SequenceChange { Name = "HoG" });
            }
        }

        public void OnPantheonStatsQuery(PantheonStatsQuery msg)
        {
            // Detect pantheon sequences by stats queries.
            // When the pantheon index is null (i.e. unidentified pantheon), the sequence name becomes null as returned by `GodhomeUtils.GetPantheonSequenceName()`.
            _bus.Put(new SequenceChange { Name = GodhomeUtils.GetPantheonSequenceName(msg.PantheonIndex, msg.PantheonAttribute) });
        }
    }
}
