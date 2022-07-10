using HutongGames.PlayMaker;
using UnityEngine;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class BossStateChange : IMessage
    {
        public GameObject BossGO { get; set; }
        public PlayMakerFSM FSM { get; set; }
        public FsmState State { get; set; }

        public override string ToString()
        {
            return $"Boss {BossGO.name}'s state changed to {State.Name}";
        }
    }
}
