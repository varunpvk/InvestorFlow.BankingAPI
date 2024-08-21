using IF.Application.Abstractions;
using IF.Domain.Enums;

namespace IF.Application.BankingService.Commands
{
    public class WithdrawMoneyCommand : ICommand
    {
        public Guid CustomerId { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }
        public AccountType Type { get; private set; }

        public WithdrawMoneyCommand(Guid customerId, decimal amount, string currency, AccountType type)
        {
            CustomerId = customerId;
            Amount = amount;
            Currency = currency;
            Type = type;
        }
    }
}
