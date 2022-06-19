using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GodhomeWinLossTracker.MessageBus.Handlers;
using GodhomeWinLossTracker.MessageBus.Messages;

namespace GodhomeWinLossTracker.MessageBus
{
    internal class TheMessageBus
    {
        public void Initialize(GodhomeWinLossTracker mod)
        {
            _logger = mod;

            _handlers = new IHandler[]
            {
                new BossChangeDetector(),
                new Logger(),
                new SequenceChangeDetector(),
                new WinLossGenerator(),
                new WinLossTracker(mod),
            };

            _messages = new();

            this.Put(new BusEvent { Event = "initialized" });
        }

        public void Put(IMessage message)
        {
            _messages.Enqueue(message);

            while (_messages.Count() != 0)
            {
                IMessage msg = _messages.Dequeue();
                foreach (IHandler handler in _handlers)
                {
                    handler.OnMessage(this, _logger, msg);
                }
            }
        }

        private IHandler[] _handlers;
        private Queue<IMessage> _messages;
        private Modding.Loggable _logger;
    }
}
