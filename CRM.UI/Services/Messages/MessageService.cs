namespace CRM.UI.Services.Messages
{
    public class MessageService : IMessageService
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new();

        public void Send<TMessage>(TMessage message) where TMessage : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = typeof(TMessage);

            if (_subscribers.TryGetValue(messageType, out var handlers))
            {
                foreach (var handler in handlers.ToList())
                {
                    (handler as Action<TMessage>)?.Invoke(message);
                }
            }
        }

        public void Subscribe<TMessage>(Action<TMessage> action) where TMessage : class
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var messageType = typeof(TMessage);

            if (!_subscribers.ContainsKey(messageType))
            {
                _subscribers[messageType] = new List<Delegate>();
            }

            _subscribers[messageType].Add(action);
        }

        public void Unsubscribe<TMessage>(Action<TMessage> action) where TMessage : class
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var messageType = typeof(TMessage);

            if (_subscribers.TryGetValue(messageType, out var handlers))
            {
                handlers.Remove(action);

                if (handlers.Count == 0)
                {
                    _subscribers.Remove(messageType);
                }
            }
        }
    }
}
