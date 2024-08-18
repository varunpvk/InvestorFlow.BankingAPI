namespace IF.Domain.ValueObjects
{
    public sealed record Balance
    {
        public Balance(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public decimal Amount { get; private set; }
        public string Currency { get; private set; }
    }
}
