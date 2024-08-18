using IF.Application.Abstractions;

namespace IF.Application.BankingService.Commands
{
    public class DeleteBankAccountCommand : ICommand
    {
        public Guid CustomerId { get; private set; }

        public DeleteBankAccountCommand(Guid customerId)
        {
            CustomerId = customerId;
        }
    }
}
