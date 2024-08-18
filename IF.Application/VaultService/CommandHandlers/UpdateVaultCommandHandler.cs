using IF.Application.Abstractions;
using IF.Application.VaultService.Commands;
using IF.Domain;
using IF.Domain.Entities;
using IF.Domain.ErrorMessages;
using IF.Infrastructure.BankingRepository;

namespace IF.Application.VaultService.CommandHandlers
{
    public class UpdateVaultCommandHandler : ICommandHandler<UpdateVaultCommand, Result<bool, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateVaultCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool, ValidationError>> HandleAsync(UpdateVaultCommand command)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                var vault = await _unitOfWork.Vaults.GetAsync(command.Id);

                if (vault != null)
                {
                    var vaultInfo = new Vault(Guid.Parse(vault.Id), Guid.Parse(vault.AccountId));
                    vaultInfo.UpdateVault(command.Amount, command.Currency);
                    var success = await _unitOfWork.Vaults.UpdateAsync(vaultInfo);

                    if (success)
                    {
                        _unitOfWork.Commit();
                        return Result<bool, ValidationError>.Success(true);
                    }
                }

                _unitOfWork.Rollback();
                return Result<bool, ValidationError>.Failure(new ValidationError("Failed to update vault"));
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result<bool, ValidationError>.Failure(new ValidationError($"Exception while updating vault: {ex.Message}"));
            }
        }
    }
}
