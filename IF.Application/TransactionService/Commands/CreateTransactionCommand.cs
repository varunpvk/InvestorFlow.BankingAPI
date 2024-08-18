using IF.Application.Abstractions;
using IF.Domain.Enums;
using IF.Domain.ValueObjects;

namespace IF.Application.TransactionService.Commands
{
    public class CreateTransactionCommand : ICommand
    {
        public Guid Id { get; private set; }
        public Guid CustomerAccountId { get; private set; }
        public TransactionDetails Details { get; private set; }
        public DateTime TransactionDateUtc { get; private set; }

        public CreateTransactionCommand(Guid id, Guid customerAccountId, TransactionDetails details, DateTime transactionDateUtc)
        {
            Id = id;
            CustomerAccountId = customerAccountId;
            Details = details;
            TransactionDateUtc = transactionDateUtc;
        }
    }
}
