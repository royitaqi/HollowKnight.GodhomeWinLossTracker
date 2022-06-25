using System.Text;

namespace UnitTests
{
    internal static class TestUtils
    {
        public static string ConvertMessagesToString(IEnumerable<IMessage> messages)
        {
            StringBuilder sb = new();
            sb.AppendLine();
            foreach (var msg in messages)
            {
                sb.AppendLine($"    {msg.GetType().Name}: {msg}");
            }
            return sb.ToString();
        }

        public class MessageBusTestCase
        {
            public string Name { get; set; }
            public IEnumerable<IHandler> Handlers { get; set; }
            public IEnumerable<IMessage> InputMessages { get; set; }
            public IEnumerable<IMessage> ExpectedMessages { get; set; }
            public G.AssertionFailedException ExpectedException { get; set; }
        }

        public static void TestMessageBus(IEnumerable<MessageBusTestCase> testCases)
        {
            foreach (var testCase in testCases)
            {
                TestMessageBus(testCase);
            }
        }

        public static void TestMessageBus(MessageBusTestCase testCase)
        {
            // Only one of the these can be nonnull.
            Assert.IsTrue((testCase.ExpectedMessages == null) != (testCase.ExpectedException == null));

            if (testCase.ExpectedMessages != null)
            {
                TestMessageBus(testCase.Name, testCase.Handlers, testCase.InputMessages, testCase.ExpectedMessages);
            }
            else
            {
                TestMessageBus(testCase.Name, testCase.Handlers, testCase.InputMessages, testCase.ExpectedException);
            }
        }

        public static void TestMessageBus(string testName, IEnumerable<IHandler> handlers, IEnumerable<IMessage> inputMessages, IEnumerable<IMessage> expectedMessages)
        {
            IEnumerable<IMessage> outputMessages = RunMessageBus(testName, handlers, inputMessages);

            // Verify output messages
            Assert.AreEqual(
                TestUtils.ConvertMessagesToString(new[] { new BusEvent { Event = "initialized" } }.Concat(expectedMessages)),
                TestUtils.ConvertMessagesToString(outputMessages),
                testName
            );
        }

        public static void TestMessageBus(string testName, IEnumerable<IHandler> handlers, IEnumerable<IMessage> inputMessages, G.AssertionFailedException expectedException)
        {
            try
            {
                RunMessageBus(testName, handlers, inputMessages);
                throw new AssertFailedException($"Should throw AssertionFailedException(\"{expectedException.Message}\")");
            }
            catch (G.AssertionFailedException ex)
            {
                Assert.AreEqual(expectedException.Message, ex.Message, "AssertionFailedException thrown with unexpected message");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static IEnumerable<IMessage> RunMessageBus(string testName, IEnumerable<IHandler> handlers, IEnumerable<IMessage> inputMessages)
        {
            // Create objects
            Mock<G.IGodhomeWinLossTracker> mod = new();
            MessageRecorder recorder = new();
            TheMessageBus bus = new(mod.Object, handlers.Concat(new[] { recorder }));

            // Feed input messages
            foreach (var inputMessage in inputMessages)
            {
                bus.Put(inputMessage);
            }

            return recorder.Messages;
        }
    }
}