namespace IF.Domain.DTOs
{
    public class VaultDTO
    {
        public string Id { get; private set; }
        public string AccountId { get; private set; }
        public decimal CurrentBalance { get; set; }
        public string Currency { get; private set; }

        public VaultDTO() { }

        public VaultDTO(string id, string accountId, decimal currentBalance, string currency)
        {
            Id = id;
            AccountId = accountId;
            CurrentBalance = currentBalance;
            Currency = currency;
        }
    }
}
