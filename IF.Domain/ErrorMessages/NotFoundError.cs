namespace IF.Domain.ErrorMessages
{
    public class NotFoundError
    {
        public string Message { get; }

        public NotFoundError(string message)
        {
            Message = message;
        }
    }
}
