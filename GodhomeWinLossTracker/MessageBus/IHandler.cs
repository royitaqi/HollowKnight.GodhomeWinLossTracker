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
        public void OnMessage(TheMessageBus bus, Modding.ILogger logger, IMessage msg)
        {
            var msgType = msg.GetType();
            
            MethodInfo method = GetType().GetMethod("OnMessage");
            if (method.IsGenericMethodDefinition)
            {
                MethodInfo generic = method.MakeGenericMethod(msgType);
                try
                {
                    generic.Invoke(this, new object[] { bus, logger, msg });
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            }
            else
            {
                try
                {
                    method.Invoke(this, new object[] { bus, logger, msg });
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            }
        }
    }
}
