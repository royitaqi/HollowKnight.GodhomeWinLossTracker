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
        }

        public static void TestMessageBus(MessageBusTestCase testCase)
        {
            TestMessageBus(testCase.Name, testCase.Handlers, testCase.InputMessages, testCase.ExpectedMessages);
        }

        public static void TestMessageBus(string testName, IEnumerable<IHandler> handlers, IEnumerable<IMessage> inputMessages, IEnumerable<IMessage> expectedMessages)
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

            // Verify output messages
            Assert.AreEqual(
                TestUtils.ConvertMessagesToString(new[] { new BusEvent { Event = "initialized" } }.Concat(expectedMessages)),
                TestUtils.ConvertMessagesToString(recorder.Messages),
                testName
            );
        }

        public static void TestMessageBus(IEnumerable<MessageBusTestCase> testCases)
        {
            foreach (var testCase in testCases)
            {
                TestMessageBus(testCase);
            }
        }
    }
}