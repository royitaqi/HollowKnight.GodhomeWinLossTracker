using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GodhomeWinLossTracker.MessageBus
{
    internal abstract class Handler
    {
        public virtual void OnMessage(TheMessageBus bus, Modding.ILogger logger, IMessage msg)
        {
            var msgType = msg.GetType();

            // Getting message processing method by name.
            // This can be improved to get method by parameter types.
            string methodName = $"On{msgType.Name}";
            MethodInfo method = GetType().GetMethod(methodName);

            if (method != null)
            {
                try
                {
                    method.Invoke(this, new object[] { bus, logger, msg });
                }
                catch (TargetInvocationException ex)
                {
                    throw new ApplicationException($"Exception thrown while trying to invoke {GetType().Name}.{methodName}() to process {msgType.Name} \"{msg}\": {ex.Message}", ex.InnerException);
                }
            }
        }
    }
}
