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
        public GameObject Enemy { get; set; }
        public override string ToString()
        {
            return $"Enemy enabled: name={Enemy.name} max_hp={Enemy.GetComponent<HealthManager>().hp}";
        }
    }
}
