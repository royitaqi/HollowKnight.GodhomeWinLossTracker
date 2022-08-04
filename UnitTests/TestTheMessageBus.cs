namespace UnitTests
{
    [TestClass]
    public class TestTheMessageBus
    {
        [TestMethod]
        public void TestSingleMessage()
        {
            var handlers = new Handler[] { };
            var inputMessages = new[] { new BusEvent { Event = "test" } };
            var expectedMessages = new[] { new BusEvent { Event = "test" } };

            TestUtils.TestMessageBus(new TestUtils.MessageBusTestCase {
                Name = "An non-effect message should pass through message bus",
                HandlersCreator = _ => handlers,
                InputMessages = inputMessages,
                ExpectedMessages = expectedMessages,
            });
        }


        private class Echo : Handler
        {
            public Echo(string id, int maxLen)
            {
                _id = id;
                _maxLen = maxLen;
            }

            public void OnBusEvent(BusEvent msg)
            {
                string evt = msg.Event;

                // Echo message + id if length < 3
                if (evt.Length < _maxLen)
                {
                    _bus.Put(new BusEvent { Event = evt + _id });
                }
            }

            private readonly string _id;
            private readonly int _maxLen;
        }

        [TestMethod]
        public void TestQueueProcessingOrder()
        {
            var handlers = new Handler[] {
                new Echo("1", 3),
                new Echo("2", 3),
            };
            var inputMessages = new[] { new BusEvent { Event = "0" }, new BusEvent { Event = "1" } };
            var expectedMessages = new[] {
                // 0 be put onto bus
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
                // ---
                // 1 be put onto bus
                new BusEvent { Event = "1" },
                // 1 processed by echo 1
                new BusEvent { Event = "11" },
                // 1 processed by echo 2
                new BusEvent { Event = "12" },
                // 11 processed by echo 1
                new BusEvent { Event = "111" },
                // 11 processed by echo 2
                new BusEvent { Event = "112" },
                // 12 processed by echo 1
                new BusEvent { Event = "121" },
                // 12 processed by echo 2
                new BusEvent { Event = "122" },
                // 111, 112, 121, 122 processed by echo 1 and 2
            };

            TestUtils.TestMessageBus(new TestUtils.MessageBusTestCase {
                Name = "Messages should be processed FIFO, each time by all handlers",
                HandlersCreator = _ => handlers,
                InputMessages = inputMessages,
                ExpectedMessages = expectedMessages,
            });
        }

        [TestMethod]
        public void TestLoadUnload()
        {
            var handlers = new Handler[] {
                new Echo("A", 2),
            };
            var inputMessages = new IMessage[] {
                new BusEvent { Event = "0" },
                new BusCommand { Command = BusCommand.Commands.Load },
                new BusEvent { Event = "1" },
                new BusCommand { Command = BusCommand.Commands.Unload },
                new BusEvent { Event = "2" },
                new BusCommand { Command = BusCommand.Commands.Load },
                new BusEvent { Event = "3" },
                new BusCommand { Command = BusCommand.Commands.Unload },
                new BusEvent { Event = "4" },
            };
            var expectedMessages = new IMessage[] {
                new BusEvent { Event = "0" },
                new BusEvent { Event = "0A" },
                new BusCommand { Command = BusCommand.Commands.Load },
                new BusEvent { Event = "1" },
                new BusEvent { Event = "1A" },
                new BusCommand { Command = BusCommand.Commands.Unload },
                new BusEvent { Event = "2" },
                // No echo for "2"
                new BusCommand { Command = BusCommand.Commands.Load },
                new BusEvent { Event = "3" },
                new BusEvent { Event = "3A" },
                new BusCommand { Command = BusCommand.Commands.Unload },
                new BusEvent { Event = "4" },
                // No echo for "4"
            };

            TestUtils.TestMessageBus(new TestUtils.MessageBusTestCase
            {
                Name = "Messages should be echo-ed when Echo is loaded, should not be echo-ed when Echo is unloaded",
                HandlersCreator = _ => handlers,
                InputMessages = inputMessages,
                ExpectedMessages = expectedMessages,
            });
        }
    }
}