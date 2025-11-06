namespace CRM.Shared.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) không tồn tại.") { }
    }
}
