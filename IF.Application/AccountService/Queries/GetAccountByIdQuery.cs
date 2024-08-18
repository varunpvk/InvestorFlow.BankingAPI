using IF.Application.Abstractions;
using IF.Domain;
using IF.Domain.DTOs;
using IF.Domain.Entities;
using IF.Domain.ErrorMessages;

namespace IF.Application.AccountService.Queries
{
    public class GetAccountByIdQuery : IQuery<Result<AccountDTO, NotFoundError>>
    {
        public Guid Id { get; }

        public GetAccountByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
