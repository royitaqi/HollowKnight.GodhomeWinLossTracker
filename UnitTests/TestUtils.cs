using GodhomeWinLossTracker.Utils;

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
            public Func<G.IGodhomeWinLossTracker, IEnumerable<Handler>> HandlersCreator { get; set; }
            public Func<IEnumerable<IMessage>, IEnumerable<IMessage>> OutputMessageFilter { get; set; }
            public IEnumerable<IMessage> InputMessages { get; set; }
            public IEnumerable<IMessage> ExpectedMessages { get; set; }
            public Exception ExpectedException { get; set; }

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
                // Run message bus and get output messages
                IEnumerable<IMessage> outputMessages = RunMessageBus(testCase.Name, testCase.GlobalData, testCase.LocalData, testCase.FolderData, testCase.HandlersCreator, testCase.InputMessages);

                // Filter output messages if specified
                if (testCase.OutputMessageFilter != null)
                {
                    outputMessages = testCase.OutputMessageFilter(outputMessages);
                }

                // Verify output messages
                Assert.AreEqual(
                    TestUtils.ConvertMessagesToString(testCase.ExpectedMessages),
                    TestUtils.ConvertMessagesToString(outputMessages),
                    testCase.ToString()
                );
            }
            else
            {
                try
                {
                    RunMessageBus(testCase.Name, testCase.GlobalData, testCase.LocalData, testCase.FolderData, testCase.HandlersCreator, testCase.InputMessages);
                    throw new AssertFailedException($"Should throw {testCase.ExpectedException.GetType().Name}(\"{testCase.ExpectedException.Message}\"). {testCase}");
                }
                catch (Exception ex)
                {
                    // Look for expected exception in inner exceptions, recursively.
                    var innerEx = ex;
                    do
                    {
                        if (innerEx.GetType() == testCase.ExpectedException.GetType() && innerEx.Message == testCase.ExpectedException.Message)
                        {
                            // Found the expected exception. Good.
                            return;
                        }
                        ex = innerEx;
                        innerEx = innerEx.InnerException;
                    }
                    while (innerEx != null);

                    // Didn't find expected exception. Bad. Report the innermost exception.
                    throw new AssertFailedException($"Should throw {testCase.ExpectedException.GetType().Name}(\"{testCase.ExpectedException.Message}\"). Caught {ex.GetType().Name}(\"{ex.Message}\"). {testCase}", ex);
                }
            }
        }

        public static string TestLocalizer(string str)
        {
            return str.Substring(str.LastIndexOf('/') + 1);
        }

        private static IEnumerable<IMessage> RunMessageBus(
            string testName,
            G.GlobalData globalData,
            G.LocalData localData,
            G.FolderData folderData,
            Func<G.IGodhomeWinLossTracker, IEnumerable<Handler>> handlersCreator,
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
                sb.AppendLine($"    {msg.GetType().Name}:{ConvertMessageToString(msg)}");
            }
            return sb.ToString();
        }

        internal static string ConvertMessageToString(IMessage msg)
        {
            var skipNames = new HashSet<string>
            {
                "Timestamp",
                "RecordSource",
            };

            StringBuilder sb = new();
            foreach (var p in msg.GetType().GetProperties())
            {
                if (skipNames.Contains(p.Name)) continue;
                sb.AppendFormat(" {0}={1}", p.Name, p.GetValue(msg));
            }
            foreach (var f in msg.GetType().GetFields())
            {
                if (skipNames.Contains(f.Name)) continue;
                sb.AppendFormat(" {0}={1}", f.Name, f.GetValue(msg));
            }
            return sb.ToString();
        }
    }
}