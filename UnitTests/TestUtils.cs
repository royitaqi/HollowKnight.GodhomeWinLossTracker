namespace UnitTests
{
    internal static class TestUtils
    {
        public class MessageBusTestCase
        {
            public string Name { get; set; }
            public G.GlobalData GlobalData { get; set; }
            public G.LocalData LocalData { get; set; }
            public G.FolderData FolderData { get; set; }
            public Func<G.IGodhomeWinLossTracker, IEnumerable<IHandler>> HandlersCreator { get; set; }
            public IEnumerable<IMessage> InputMessages { get; set; }
            public IEnumerable<IMessage> ExpectedMessages { get; set; }
            public G.AssertionFailedException ExpectedException { get; set; }

            public override string ToString()
            {
                StringBuilder sb = new();
                sb.AppendLine();
                sb.AppendLine($"Test Case = {Name}");
                if (GlobalData != null)
                {
                    sb.AppendLine($"GlobalData = {JsonConvert.SerializeObject(GlobalData, Formatting.Indented)}");
                }
                if (LocalData != null)
                {
                    sb.AppendLine($"LocalData = {JsonConvert.SerializeObject(LocalData, Formatting.Indented)}");
                }
                if (FolderData != null)
                {
                    sb.AppendLine($"FolderData = {JsonConvert.SerializeObject(FolderData, Formatting.Indented)}");
                }
                return sb.ToString();
            }
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
                IEnumerable<IMessage> outputMessages = RunMessageBus(testCase.Name, testCase.GlobalData, testCase.LocalData, testCase.FolderData, testCase.HandlersCreator, testCase.InputMessages);

                // Verify output messages
                Assert.AreEqual(
                    TestUtils.ConvertMessagesToString(new[] { new BusEvent { Event = "initialized" } }.Concat(testCase.ExpectedMessages)),
                    TestUtils.ConvertMessagesToString(outputMessages),
                    testCase.ToString()
                );
            }
            else
            {
                try
                {
                    RunMessageBus(testCase.Name, testCase.GlobalData, testCase.LocalData, testCase.FolderData, testCase.HandlersCreator, testCase.InputMessages);
                    throw new AssertFailedException($"Should throw AssertionFailedException(\"{testCase.ExpectedException.Message}\"). {testCase}");
                }
                catch (G.AssertionFailedException ex)
                {
                    Assert.AreEqual(testCase.ExpectedException.Message, ex.Message, $"AssertionFailedException thrown with unexpected message. {testCase}");
                }
                catch (Exception ex)
                {
                    throw new AssertFailedException(testCase.ToString(), ex);
                }
            }
        }

        private static IEnumerable<IMessage> RunMessageBus(
            string testName,
            G.GlobalData globalData,
            G.LocalData localData,
            G.FolderData folderData,
            Func<G.IGodhomeWinLossTracker, IEnumerable<IHandler>> handlersCreator,
            IEnumerable<IMessage> inputMessages
        )
        {
            // Create objects
            Mock<G.IGodhomeWinLossTracker> mod = new();
            if (globalData != null)
            {
                mod.SetupGet(m => m.globalData).Returns(globalData);
            }
            if (localData != null)
            {
                mod.SetupGet(m => m.localData).Returns(localData);
            }
            if (folderData != null)
            {
                mod.SetupGet(m => m.folderData).Returns(folderData);
            }

            MessageRecorder recorder = new();
            var handlers = handlersCreator(mod.Object).Concat(new[] { recorder });
            TheMessageBus bus = new(mod.Object, handlers);

            // Feed input messages
            foreach (var inputMessage in inputMessages)
            {
                bus.Put(inputMessage);
            }

            return recorder.Messages;
        }

        private static string ConvertMessagesToString(IEnumerable<IMessage> messages)
        {
            StringBuilder sb = new();
            sb.AppendLine();
            foreach (var msg in messages)
            {
                sb.AppendLine($"    {msg.GetType().Name}: {msg}");
            }
            return sb.ToString();
        }
    }
}