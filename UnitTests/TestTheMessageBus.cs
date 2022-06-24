namespace UnitTests
{
    [TestClass]
    public class TestTheMessageBus
    {
        [TestMethod]
        public void TestSingleMessage()
        {
            //Mock<G.GodhomeWinLossTracker> mod = new();
            //mod.setup(o => o.bar()).Returns(/*mock implementation*/);
            G.GodhomeWinLossTracker mod = new();

            IMessage testMessage = new BusEvent { Event = "test" };

            HaveMessagesInOrder testHandler = new HaveMessagesInOrder(new List<IMessage>
            {
                testMessage,
            });

            TheMessageBus bus = new(mod);
            bus.Subscribe(testHandler);

            bus.Put(testMessage);
            testHandler.Validate();
        }
    }
}