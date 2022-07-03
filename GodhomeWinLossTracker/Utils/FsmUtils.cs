using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodhomeWinLossTracker.Utils
{
    internal static class FsmUtils
    {
        // Enable this can make the FPS drop from 270 to 200
        public static void Initialize()
        {
            On.PlayMakerFSM.Start += PlayMakerFSM_Start;
            On.PlayMakerFSM.SetState += PlayMakerFSM_SetState;
            On.PlayMakerFSM.Update += PlayMakerFSM_Update;
            On.PlayMakerFSM.SendEvent += PlayMakerFSM_SendEvent;
            On.PlayMakerFSM.SendRemoteFsmEvent += PlayMakerFSM_SendRemoteFsmEvent;
            On.PlayMakerFSM.ChangeState_FsmEvent += PlayMakerFSM_ChangeState_FsmEvent;
        }

        private static void PlayMakerFSM_Start(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
        {
            DevUtils.CountedLog($"PlayMakerFSM_Start: GO={self.gameObject.name} FsmName={self.FsmName}");
            orig(self);
        }

        private static void PlayMakerFSM_SetState(On.PlayMakerFSM.orig_SetState orig, PlayMakerFSM self, string stateName)
        {
            DevUtils.CountedLog($"PlayMakerFSM_SetState: GO={self.gameObject.name} FsmName={self.FsmName} stateName={stateName}");
            orig(self, stateName);
        }

        private static void PlayMakerFSM_Update(On.PlayMakerFSM.orig_Update orig, PlayMakerFSM self)
        {
            DevUtils.CountedLog($"PlayMakerFSM_Update: GO={self.gameObject.name} FsmName={self.FsmName}");
            orig(self);
        }

        private static void PlayMakerFSM_SendEvent(On.PlayMakerFSM.orig_SendEvent orig, PlayMakerFSM self, string eventName)
        {
            DevUtils.CountedLog($"PlayMakerFSM_SendEvent: GO={self.gameObject.name} FsmName={self.FsmName} eventName={eventName}");
            orig(self, eventName);
        }

        private static void PlayMakerFSM_SendRemoteFsmEvent(On.PlayMakerFSM.orig_SendRemoteFsmEvent orig, PlayMakerFSM self, string eventName)
        {
            DevUtils.CountedLog($"PlayMakerFSM_SendRemoteFsmEvent: GO={self.gameObject.name} FsmName={self.FsmName} eventName={eventName}");
            orig(self, eventName);
        }

        private static void PlayMakerFSM_ChangeState_FsmEvent(On.PlayMakerFSM.orig_ChangeState_FsmEvent orig, PlayMakerFSM self, HutongGames.PlayMaker.FsmEvent fsmEvent)
        {
            DevUtils.CountedLog($"PlayMakerFSM_ChangeState_FsmEvent: GO={self.gameObject.name} FsmName={self.FsmName} eventName={fsmEvent.Name}");
            orig(self, fsmEvent);
        }
    }
}
