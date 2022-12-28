namespace UnitTests
{
    [TestClass]
    public class TestHandler
    {
        private class TestException : Exception
        {
            public TestException(string message) : base(message) { }
        }

        private class AlwaysThrow : Handler
        {
            public AlwaysThrow(string error)
            {
                _error = error;
            }

            public void OnBusEvent(BusEvent msg)
            {
                throw new TestException(_error);
            }

            private readonly string _error;
        }

        [TestMethod]
        public void TestExceptionThrownFromHandler()
        {
            var handlers = new Handler[] { new AlwaysThrow("Test Message") };
            var inputMessages = new[] { new BusEvent() };

            TestUtils.TestMessageBus(new TestUtils.MessageBusTestCase
            {
                Name = "An non-effect message should pass through message bus",
                HandlersCreator = _ => handlers,
                InputMessages = inputMessages,
                ExpectedException = new TestException("Test Message"),
            });
        }
    }
}