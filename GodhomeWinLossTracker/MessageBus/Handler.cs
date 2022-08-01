using System;
using System.Reflection;
using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus
{
    internal class Handler
    {
        public virtual void Load(IGodhomeWinLossTracker mod, TheMessageBus bus, Modding.ILogger logger)
        {
            logger.LogMod($"Loading {GetType().Name}");
            _mod = mod;
            _bus = bus;
            _logger = logger;
            _loaded = true;
        }

        public virtual void Unload()
        {
            _logger.LogMod($"Unloading {GetType().Name}");
            _loaded = false;
        }

        public virtual void OnBusCommand(BusCommand cmd)
        {
            if (cmd.Command == BusCommand.Commands.Load)
            {
                // Only load if the handler hasn't been loaded
                if (!_loaded)
                {
                    Load(_mod, _bus, _logger);
                }
            }
            else if (cmd.Command == BusCommand.Commands.Unload)
            {
                // Only unload if the handler has been loaded
                if (_loaded)
                {
                    Unload();
                }
            }
        }

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
        protected bool _loaded = false;
    }
}
