using IF.Application.Abstractions;
using IF.Application.VaultService.Queries;
using IF.Domain;
using IF.Domain.DTOs;
using IF.Domain.ErrorMessages;
using IF.Infrastructure.BankingRepository;

namespace IF.Application.VaultService.QueryHandler
{
    public class GetVaultByIdQueryHandler : IQueryHandler<GetVaultByIdQuery, Result<VaultDTO, NotFoundError>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetVaultByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<VaultDTO, NotFoundError>> HandleAsync(GetVaultByIdQuery query)
        {
            try
            {
                var vault = await _unitOfWork.Vaults.GetAsync(query.Id);

                if (vault != null)
                {
                    return Result<VaultDTO, NotFoundError>.Success(vault);
                }

                return Result<VaultDTO, NotFoundError>.Failure(new NotFoundError("Vault not found"));
            }
            catch (Exception ex)
            {
                return Result<VaultDTO, NotFoundError>.Failure(new NotFoundError($"Exception while getting Vault: {ex.Message}"));
            }
        }
    }
}
