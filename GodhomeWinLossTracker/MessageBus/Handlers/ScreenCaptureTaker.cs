using System;
using System.IO;
using System.Text;
using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;
using UnityEngine;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class ScreenCaptureTaker : Handler
    {
        public void OnSequenceChange(SequenceChange msg)
        {
            _seq = msg;
        }

        public void OnBossChange(BossChange msg)
        {
            _boss = msg;
        }

        public void OnPhaseChange(PhaseChange msg)
        {
            _phase = msg;
        }

        public void OnRawHit(RawHit msg)
        {
            _hit = msg;
            MayTakeScreenCapture();
        }

        private void MayTakeScreenCapture()
        {
            if (!_mod.globalData.AutoScreenCapture)
            {
                return;
            }

            int taking = 0;
            _logger.LogModFine($"ScreenCaptureTaker: going through {_mod.globalData.AutoScreenCaptureConfigs.Count} configs");
            for (int i = 0; i < _mod.globalData.AutoScreenCaptureConfigs.Count; i++)
            {
                var config = _mod.globalData.AutoScreenCaptureConfigs[i];
                if (config.Trigger == AutoScreenCaptureConfig.Triggers.Hits)
                {
                    var sb = new StringBuilder("Hit");

                    // Filter through all conditions and build up filename
                    if (config.SequenceName != null)
                    {
                        if (_seq?.Name != config.SequenceName)
                        {
                            _logger.LogModFine($"ScreenCaptureTaker: config #{i} filtered out because of mismatching sequence name (expecting \"{config.SequenceName}\", got \"{_seq?.Name}\")");
                            continue;
                        }
                        sb.Append("__" + config.SequenceName);
                    }
                    if (config.SceneName != null)
                    {
                        if (_boss?.SceneName != config.SceneName)
                        {
                            _logger.LogModFine($"ScreenCaptureTaker: config #{i} filtered out because of mismatching scene name (expecting \"{config.SceneName}\", got \"{_boss?.SceneName}\")");
                            continue;
                        }
                        sb.Append("__" + config.SceneName);
                    }
                    if (config.DamageSource != null)
                    {
                        if (_hit?.DamageSource != config.DamageSource)
                        {
                            _logger.LogModFine($"ScreenCaptureTaker: config #{i} filtered out because of mismatching damage source (expecting \"{config.DamageSource}\", got \"{_hit?.DamageSource}\")");
                            continue;
                        }
                        sb.Append("__" + config.DamageSource);
                    }
                    if (config.BossPhase != 0)
                    {
                        if (_phase?.Phase != config.BossPhase)
                        {
                            _logger.LogModFine($"ScreenCaptureTaker: config #{i} filtered out because of mismatching boss phase (expecting \"{config.BossPhase}\", got \"{_phase?.Phase}\")");
                            continue;
                        }
                        sb.Append($"__Phase_{config.BossPhase}");
                    }

                    if (++taking > 1)
                    {
                        _logger.LogModWarn($"Attempt to take more than one screen capture from a single event ({taking})");
                    }
                    else
                    {
                        string path = Path.Combine(SaveLoad.ModSaveDirectory, sb.ToString().Replace(" ", "_") + "__" + DateTime.Now.ToString("yyyyMMdd.HHmmss") + ".png");
                        _logger.LogModFine($"ScreenCaptureTaker: saving screen capture into {path}");
                        ScreenCapture.CaptureScreenshot(path);
                    }
                }
            }
        }

        private SequenceChange _seq;
        private BossChange _boss;
        private PhaseChange _phase;
        private RawHit _hit;
    }
}
