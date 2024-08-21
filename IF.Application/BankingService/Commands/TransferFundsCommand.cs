using IF.Application.Abstractions;
using IF.Domain.Enums;

namespace IF.Application.BankingService.Commands
{
    public class TransferFundsCommand : ICommand
    {
        public Guid CustomerId { get; private set; }
        public Guid DestinationAccountId { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }
        public AccountType Type { get; private set; }

        public TransferFundsCommand(Guid customerId, AccountType type, Guid destinationAccountId, decimal amount, string currency)
        {
            CustomerId = customerId;
            DestinationAccountId = destinationAccountId;
            Amount = amount;
            Currency = currency;
            Type = type;
        }
    }
}
