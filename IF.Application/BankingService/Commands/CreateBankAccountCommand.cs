using IF.Application.Abstractions;
using IF.Domain.Enums;

namespace IF.Application.BankingService.Commands
{
    public class CreateBankAccountCommand : ICommand
    {
        public Guid CustomerId { get; private set; }
        public AccountType Type { get; private set; }
        public DateTime CreationDate { get; private set; }

        public CreateBankAccountCommand(Guid customerId, AccountType type, DateTime creationDate)
        {
            CustomerId = customerId;
            Type = type;
            CreationDate = creationDate;
        }
    }
}
