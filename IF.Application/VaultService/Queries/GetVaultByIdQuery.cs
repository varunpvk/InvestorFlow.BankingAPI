using IF.Application.Abstractions;
using IF.Domain;
using IF.Domain.DTOs;
using IF.Domain.ErrorMessages;

namespace IF.Application.VaultService.Queries
{
    public class GetVaultByIdQuery : IQuery<Result<VaultDTO, NotFoundError>>
    {
        public Guid Id { get; }

        public GetVaultByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
