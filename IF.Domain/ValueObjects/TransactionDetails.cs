using IF.Domain.Enums;

namespace IF.Domain.ValueObjects
{
    public sealed record TransactionDetails
    {
        public TransactionDetails(
            TransactionType type,
            decimal amount,
            string currency)
        {
            Type = type;
            Amount = amount;
            Currency = currency;
            Description = GetDescription(type);
        }

        public TransactionType Type { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }
        public string Description { get; private set; }

        private string GetDescription(TransactionType type)
        {
            return type switch
            {
                TransactionType.Credit => "Money Credited",
                TransactionType.Debit => "Money Debited",
                TransactionType.Deposit => "Money Deposited",
                TransactionType.Withdrawal => "Money Withdrawn",
                TransactionType.TransferSent => "Money Sent",
                TransactionType.TransaferReceived => "Money Received",
                TransactionType.Interest => "Interest Added",
                TransactionType.Charge => "Charge Deducted",
                _ => "Unknown"
            };
        }
    }
}
