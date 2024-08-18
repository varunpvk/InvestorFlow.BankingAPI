using IF.Application.Abstractions;
using IF.Domain;
using IF.Domain.DTOs;
using IF.Domain.ErrorMessages;

namespace IF.Application.BankingService.Queries
{
    public class GetTransactionHistoryQuery : IQuery<Result<IDictionary<Guid, IList<TransactionDTO>>, NotFoundError>>
    {
        public Guid CustomerId { get; private set; }

        public GetTransactionHistoryQuery(Guid customerId)
        {
            CustomerId = customerId;
        }
    }
}
