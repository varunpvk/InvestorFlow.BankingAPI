using IF.Application.Abstractions;
using IF.Domain;
using IF.Domain.DTOs;
using IF.Domain.ErrorMessages;

namespace IF.Application.TransactionService.Queries
{
    public class GetTransactionsByAccountIdQuery : IQuery<Result<IList<TransactionDTO>, NotFoundError>>
    {
        public Guid AccountId { get; }

        public GetTransactionsByAccountIdQuery(Guid accountId)
        {
            AccountId = accountId;
        }
    }
}
