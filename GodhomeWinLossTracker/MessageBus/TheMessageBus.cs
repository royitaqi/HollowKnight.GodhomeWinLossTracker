using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GodhomeWinLossTracker.MessageBus.Handlers;
using GodhomeWinLossTracker.MessageBus.Messages;
using GodhomeWinLossTracker.Utils;

namespace GodhomeWinLossTracker.MessageBus
{
    internal class TheMessageBus
    {
        public TheMessageBus(IGodhomeWinLossTracker mod, IEnumerable<Handler> handlers)
        {
#if DEBUG
            foreach (Handler h in handlers)
            {
                h.Validate(mod);
            }
#endif

            _handlers = new List<Handler>(handlers);
            _messages = new();
            _processing = false;

            foreach (Handler h in handlers)
            {
                h.Load(mod, this, mod);
            }

            this.Put(new BusEvent { Event = "initialized" });
        }

        public void Put(IMessage message)
        {
            DevUtils.Assert(message != null, "Message should never be null");

            // Enqueue message
            _messages.Enqueue(message);

            // Process all messages until the queue is empty
            if (!_processing)
            {
                _processing = true;
                while (_messages.Count() != 0)
                {
                    IMessage msg = _messages.Dequeue();
                    foreach (Handler handler in _handlers)
                    {
                        handler.OnMessage(msg);
                    }
                }
                _processing = false;
            }
        }

        private List<Handler> _handlers;
        private Queue<IMessage> _messages;
        private bool _processing;
    }
}
