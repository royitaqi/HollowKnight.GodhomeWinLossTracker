using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus
{
    internal abstract class Handler
    {
        public virtual void Load(IGodhomeWinLossTracker mod, TheMessageBus bus)
        {
            _mod = mod;
            _bus = bus;
        }

        public virtual void Unload(IGodhomeWinLossTracker mod, TheMessageBus bus) { }

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

        public void Validate(Modding.ILogger logger)
        {
#if DEBUG
            logger.Log($"Validating {GetType().Name}");
#endif
            foreach (var m in GetType().GetMethods())
            {
                var ps = m.GetParameters();
#if DEBUG
                logger.Log($"Validating {GetType().Name}.{m.Name}()");
                if (ps.Length == 3)
                {
                    logger.Log($"  {ps[0].Name}: {ps[0].ParameterType.Name} - {ps[0].ParameterType == typeof(TheMessageBus)}");
                    logger.Log($"  {ps[1].Name}: {ps[1].ParameterType.Name} - {typeof(Modding.ILogger).IsAssignableFrom(ps[1].ParameterType)}");
                    logger.Log($"  {ps[2].Name}: {ps[2].ParameterType.Name} - {typeof(IMessage).IsAssignableFrom(ps[2].ParameterType)}");
                }
#endif
                if (ps.Length == 3 &&
                    ps[0].ParameterType == typeof(TheMessageBus) &&
                    typeof(Modding.ILogger).IsAssignableFrom(ps[1].ParameterType) &&
                    typeof(IMessage).IsAssignableFrom(ps[2].ParameterType))
                {
#if DEBUG
                    logger.Log($"Validating {GetType().Name}.{m.Name}()'s name");
#endif
                    string expectedTypeName = ps[2].ParameterType.Name == "IMessage" ? "Message" : ps[2].ParameterType.Name;
                    DevUtils.Assert(m.Name == $"On{expectedTypeName}", $"{GetType().Name}.{m.Name}() should be renamed to {GetType().Name}.On{expectedTypeName}()");
                }
            }
        }

        protected IGodhomeWinLossTracker _mod;
        protected TheMessageBus _bus;
    }
}
