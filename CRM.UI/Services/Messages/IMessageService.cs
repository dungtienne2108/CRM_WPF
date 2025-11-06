namespace CRM.UI.Services.Messages
{
    public interface IMessageService
    {
        void Send<TMessage>(TMessage message) where TMessage : class;
        void Subscribe<TMessage>(Action<TMessage> action) where TMessage : class;
        void Unsubscribe<TMessage>(Action<TMessage> action) where TMessage : class;
    }
}
