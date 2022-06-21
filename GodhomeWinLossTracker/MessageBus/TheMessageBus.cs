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
        public TheMessageBus(GodhomeWinLossTracker mod)
        {
            _logger = mod;

            _handlers = new List<IHandler>();
#if DEBUG
            _handlers.Add(new Logger());
#endif
            _handlers.AddRange(new IHandler[] {
                new BossChangeDetector(),
                new DisplayUpdater(),
                new SaveLoad(mod),
                new SequenceChangeDetector(),
                new WinLossGenerator(),
                new WinLossTracker(mod)
            });

            _messages = new();
            _processing = false;

            this.Put(new BusEvent { Event = "initialized" });
        }

        public void Put(IMessage message)
        {
            _messages.Enqueue(message);

            if (!_processing)
            {
                _processing = true;
                while (_messages.Count() != 0)
                {
                    IMessage msg = _messages.Dequeue();
                    foreach (IHandler handler in _handlers)
                    {
                        handler.OnMessage(this, _logger, msg);
                    }
                }
                _processing = false;
            }
        }

        private List<IHandler> _handlers;
        private Queue<IMessage> _messages;
        private Modding.Loggable _logger;
        private bool _processing;
    }
}
