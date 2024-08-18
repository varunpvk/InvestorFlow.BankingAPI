namespace IF.Domain.DTOs
{
    public class TransactionDTO
    {
        public string Id { get; private set; }
        public string CustomerAccountId { get; private set; }
        public string Type { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }
        public string Description { get; private set; }
        public DateTime TransactionDateTimeUtc { get; private set; }

        public TransactionDTO(string id, string customerAccountId, string type, decimal amount, string currency, string description, DateTime transactiionDateUtc)
        {
            Id = id;
            CustomerAccountId = customerAccountId;
            Type = type;
            Amount = amount;
            Currency = currency;
            Description = description;
            TransactionDateTimeUtc = transactiionDateUtc;
        }

        public TransactionDTO()
        {
            
        }
    }
}
