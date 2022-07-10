using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class EnemyEnabled : IMessage
    {
        public GameObject EnemyGO { get; set; }
        public override string ToString()
        {
            return $"Enemy enabled: name={EnemyGO.name} max_hp={EnemyGO.GetComponent<HealthManager>().hp}";
        }
    }
}
