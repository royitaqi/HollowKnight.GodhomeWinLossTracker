using GodhomeWinLossTracker.MessageBus.Messages;
using UnityEngine;
using Vasi;

namespace GodhomeWinLossTracker.MessageBus.Handlers
{
    internal class TKHitDetector: IHandler
    {
        public static bool Filter(FsmAwareness.Orders order, FsmAwareness.Types type, PlayMakerFSM fsm, string stateName, string eventName)
        {
            return order == FsmAwareness.Orders.After
                && type == FsmAwareness.Types.SendEvent
                && fsm.gameObject.name == "Knight"
                && fsm.FsmName == "ProxyFSM"
                && eventName == "HeroCtrl-HeroDamaged";
        }

        public void OnMessage(TheMessageBus bus, Modding.ILogger logger, IMessage message)
        {
            if (message is FsmAwareness)
            {
                var msg = message as FsmAwareness;
                if (Filter(msg.Order, msg.Type, msg.Fsm, msg.StateName, msg.EventName))
                {
                    logger.Log($"DEBUG: TK got hit: {PlayerData.instance.health + PlayerData.instance.healthBlue}");
                }
            }
        }
    }
}
