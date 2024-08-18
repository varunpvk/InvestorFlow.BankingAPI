using IF.Application.Abstractions;

namespace IF.Application.VaultService.Commands
{
    public class UpdateVaultCommand : ICommand
    {
        public Guid Id { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }

        public UpdateVaultCommand(
            Guid id,
            decimal amount,
            string currency)
        {
            Id = id;
            Amount = amount;
            Currency = currency;
        }
    }
}
