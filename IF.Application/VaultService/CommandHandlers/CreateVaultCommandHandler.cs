using IF.Application.Abstractions;
using IF.Application.VaultService.Commands;
using IF.Domain;
using IF.Domain.Entities;
using IF.Domain.ErrorMessages;
using IF.Infrastructure.BankingRepository;

namespace IF.Application.VaultService.CommandHandlers
{
    public class CreateVaultCommandHandler : ICommandHandler<CreateVaultCommand, Result<bool, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateVaultCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool, ValidationError>> HandleAsync(CreateVaultCommand command)
        {
            try
            {
                var vault = new Vault(
                    command.Id,
                    command.AccountId);
                                               
                _unitOfWork.BeginTransaction();
                var success = await _unitOfWork.Vaults.AddAsync(vault);
                if (success)
                {
                    _unitOfWork.Commit();
                    return Result<bool, ValidationError>.Success(true);
                }

                _unitOfWork.Rollback();
                return Result<bool, ValidationError>.Failure(new ValidationError("Failed to create vault"));
            }
            catch (Exception ex)
            {
                return Result<bool, ValidationError>.Failure(new ValidationError($"Exception while creating vault: {ex.Message}"));
            }
        }
    }
}
