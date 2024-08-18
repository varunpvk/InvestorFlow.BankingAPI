using IF.Application.Abstractions;
using IF.Domain;
using IF.Domain.DTOs;
using IF.Domain.ErrorMessages;

namespace IF.Application.TransactionService.Queries
{
    public class GetTransactionByIdQuery : IQuery<Result<TransactionDTO, NotFoundError>>
    {
        public Guid Id { get; }

        public GetTransactionByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
