using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus
{
    internal class Handler
    {
        public virtual void Load(IGodhomeWinLossTracker mod, TheMessageBus bus, Modding.ILogger logger)
        {
            _mod = mod;
            _bus = bus;
            _logger = logger;
        }

        public virtual void Unload() { }

        public virtual void OnMessage(IMessage msg)
        {
            var msgType = msg.GetType();

            // Getting message processing method by name.
            // The method name should have been validated by `Validate()`.
            // Using name to find the method is faster than using paramter types.
            string methodName = $"On{msgType.Name}";
            MethodInfo method = GetType().GetMethod(methodName);

            if (method != null)
            {
                try
                {
                    method.Invoke(this, new object[] { msg });
                }
                catch (TargetInvocationException ex)
                {
                    throw new ApplicationException($"Exception thrown while trying to invoke {GetType().Name}.{methodName}() to process {msgType.Name} \"{msg}\": {ex.Message}", ex.InnerException);
                }
            }
        }

        public void Validate(Modding.ILogger logger)
        {
            string handlerName = GetType().Name;
            logger.LogModFine($"Validating {handlerName}");
            foreach (var m in GetType().GetMethods())
            {
                if (!m.Name.StartsWith("On"))
                {
                    // Skip validating methods which is not named "On...".
                    continue;
                }

                // All "On..." methods should meet these requirements:
                // 1. Method has one parameter
                // 2. Parameter type is IMessage or its subclass
                // 3. Method name is either "OnMessage" (when IMessage) or "On<MessageClassName>" (when a subclass of IMessage)

                logger.LogModFine($"Validating {handlerName}.{m.Name}()");
                var ps = m.GetParameters();

                // 1
                DevUtils.Assert(ps.Length == 1, $"{handlerName}.{m.Name}() should have one parameter");

                // 2
                DevUtils.Assert(typeof(IMessage).IsAssignableFrom(ps[0].ParameterType), $"{handlerName}.{m.Name}()'s parameter should be IMessage");

                // 3
                string expectedMethodName = "On" + (ps[0].ParameterType.Name == "IMessage" ? "Message" : ps[0].ParameterType.Name);
                DevUtils.Assert(m.Name == $"{expectedMethodName}", $"{handlerName}.{m.Name}() should be renamed to {handlerName}.{expectedMethodName}()");
            }
        }

        protected IGodhomeWinLossTracker _mod;
        protected TheMessageBus _bus;
        protected Modding.ILogger _logger;
    }
}
