using UnityEngine;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class EnemyEnabled : IMessage
    {
        public GameObject EnemyGO { get; set; }
        public bool IsBoss { get; set; }
        public PlayMakerFSM FSM { get; set; }

        public override string ToString()
        {
            return $"Enemy enabled: GO.Name={EnemyGO.name} IsBoss={IsBoss} GO.HM.HP={EnemyGO.GetComponent<HealthManager>().hp} FSM={FSM?.FsmName}";
        }
    }
}
