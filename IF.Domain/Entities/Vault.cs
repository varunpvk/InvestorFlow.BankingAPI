using IF.Domain.Abstractions;
using IF.Domain.ValueObjects;

namespace IF.Domain.Entities
{
    public sealed class Vault : Entity
    {
        public Vault(
            Guid id,
            Guid accountId)
            : base(id)
        {
            AccountId = accountId;
            CurrentBalance = CreateDefaultBalance();  // Make this a value object and ensure type safety and structural correctness are validated.          
        }

        public Guid AccountId { get; private set; }
        public Balance CurrentBalance { get; set; }

        private Balance CreateDefaultBalance() => new Balance(0, "GBP");

        public void UpdateVault(decimal amount, string currency)
        {
            if (CurrentBalance != null)
                CurrentBalance = new Balance(amount, currency);
            else
                CurrentBalance = CreateDefaultBalance();
        }
    }
}
