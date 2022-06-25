using Modding;

namespace UnitTests
{
    [TestClass]
    public class TestTheMessageBus
    {
        [TestMethod]
        public void TestSingleMessage()
        {
            var handlers = new IHandler[] { };
            var inputMessages = new[] { new BusEvent { Event = "test" } };
            var expectedMessages = new[] { new BusEvent { Event = "test" } };

            TestUtils.TestMessageBus(
                "An non-effect message should pass through message bus",
                handlers,
                inputMessages,
                expectedMessages
            );
        }


        private class Echo : IHandler
        {
            public Echo(string id)
            {
                _id = id;
            }

            public void OnMessage(TheMessageBus bus, ILogger logger, IMessage msg)
            {
                if (msg is BusEvent)
                {
                    string evt = (msg as BusEvent).Event;

                    // Echo message + id if length < 3
                    if (evt.Length < 3)
                    {
                        bus.Put(new BusEvent { Event = evt + _id });
                    }
                }
            }

            private readonly string _id;
        }

        [TestMethod]
        public void TestQueueProcessingOrder()
        {
            var handlers = new IHandler[] {
                new Echo("1"),
                new Echo("2"),
            };
            var inputMessages = new[] { new BusEvent { Event = "0" } };
            var expectedMessages = new[] {
                new BusEvent { Event = "0" },
                // 0 processed by echo 1
                new BusEvent { Event = "01" },
                // 0 processed by echo 2
                new BusEvent { Event = "02" },
                // 01 processed by echo 1
                new BusEvent { Event = "011" },
                // 01 processed by echo 2
                new BusEvent { Event = "012" },
                // 02 processed by echo 1
                new BusEvent { Event = "021" },
                // 02 processed by echo 2
                new BusEvent { Event = "022" },
                // 011, 012, 021, 022 processed by echo 1 and 2
            };

            TestUtils.TestMessageBus(
                "Messages should be processed FIFO, each time by all handlers",
                handlers,
                inputMessages,
                expectedMessages
            );
        }
    }
}