using System;
using HutongGames.PlayMaker;
using SFCore.Utils;

namespace GodhomeWinLossTracker.Utils
{
    internal static class FsmUtils
    {
        // Enable this can make the FPS drop from 270 to 200
        public static void Load(Modding.ILogger logger, Func<PlayMakerFSM, bool> filter = null)
        {
            _logger = logger;
            _logger.LogMod("FsmUtils: Loading. Switching log level to 'Fine'");
            LoggingUtils.LogLevel = Modding.LogLevel.Fine;

            if (filter == null)
            {
                // Allow all logs by default
                _filter = fsm => true;
            }
            else
            {
                _filter = filter;
            }

            On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;
            On.PlayMakerFSM.Start += PlayMakerFSM_Start;
        }

        private static void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            if (_filter(self))
            {
                _logger.LogModDebug($"PlayMakerFSM_OnEnable: GO={self.gameObject.name} FsmName={self.FsmName}");
                HookAllStates(self, state =>
                {
                    _logger.LogModFine($"FSM: GO={self.gameObject.name}, FSM={self.FsmName} State={state.Name}");
                });
            }
            orig(self);
        }

        private static void PlayMakerFSM_Start(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
        {
            if (_filter(self))
            {
                _logger.LogModDebug($"PlayMakerFSM_Start: GO={self.gameObject.name} FsmName={self.FsmName}");
            }
            orig(self);
        }

        public static void HookAllStates(PlayMakerFSM fsm, Action<FsmState> act)
        {
            foreach (var state in fsm.FsmStates)
            {
                state.InsertMethod(() => act(state), 0);
            }
        }

        private static Modding.ILogger _logger;
        private static Func<PlayMakerFSM, bool> _filter;
    }
}
