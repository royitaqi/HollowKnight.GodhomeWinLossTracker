using System.Text;
using GodhomeWinLossTracker.MessageBus.Handlers;

namespace GodhomeWinLossTracker.MessageBus.Messages
{
    internal class FsmAwareness : IMessage
    {
        public enum Orders
        {
            Before,
            After,
        }

        public enum Types
        {
            // More useful
            Start,
            Update,
            SendEvent,
            // Less useful
            SetState,
            SendRemoteFsmEvent,
            ChangeState,
        }

        public static FsmAwareness Create(Orders order, Types type, PlayMakerFSM fsm, string stateName, string eventName)
        {
            if (TKHitDetector.Filter(order, type, fsm, stateName, eventName))
            {
                return new FsmAwareness
                {
                    Order = order,
                    Type = type,
                    Fsm = fsm,
                    StateName = stateName,
                    EventName = eventName,
                };
            }
            return null;
        }

        public override string ToString()
        {
            StringBuilder sb = new($"FSM {Order} {Type}: GO={Fsm.gameObject.name} FsmName={Fsm.FsmName}");
            if (StateName != null)
            {
                sb.Append($" StateName={StateName}");
            }
            if (EventName != null)
            {
                sb.Append($" EventName={EventName}");
            }
            return sb.ToString();
        }

        public Orders Order { get; set; }
        public Types Type { get; set; }
        public PlayMakerFSM Fsm { get; set; }
        public string StateName { get; set; }
        public string EventName { get; set; }
    }
}
