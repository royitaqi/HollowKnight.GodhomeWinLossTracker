using Modding;

namespace UnitTests
{
    // Validates that some of the messages received match the given ones in order.
    internal class HaveMessagesInOrder : IHandler
    {
        public HaveMessagesInOrder(List<IMessage> expectedMessages)
        {
            _expectedMessages = expectedMessages;
        }

        public void OnMessage(TheMessageBus bus, Loggable logger, IMessage msg)
        {
            // If there are messages left to be validated
            if (_nextMessageToValidate < _expectedMessages.Count)
            {
                // Try to validate the next expected message
                if (msg.ToString() == _expectedMessages[_nextMessageToValidate].ToString())
                {
                    // In case of a match, expect the next message
                    _nextMessageToValidate++;
                }
            }
            
        }

        public void Validate()
        {
            Assert.AreEqual(_expectedMessages.Count, _nextMessageToValidate);
        }

        private List<IMessage> _expectedMessages;
        private int _nextMessageToValidate = 0;
    }
}