using IF.Application.Abstractions;
using IF.Application.VaultService.Commands;
using IF.Domain;
using IF.Domain.ErrorMessages;
using IF.Infrastructure.BankingRepository;

namespace IF.Application.VaultService.CommandHandlers
{
    public class DeleteVaultCommandHandler : ICommandHandler<DeleteVaultCommand, Result<bool, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteVaultCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool, ValidationError>> HandleAsync(DeleteVaultCommand command)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                var success = await _unitOfWork.Vaults.DeleteAsync(command.Id);

                if (success)
                {
                    _unitOfWork.Commit();
                    return Result<bool, ValidationError>.Success(true);
                }

                _unitOfWork.Rollback();
                return Result<bool, ValidationError>.Failure(new ValidationError("Failed to delete vault"));
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result<bool, ValidationError>.Failure(new ValidationError($"Exception while deleting vault: {ex.Message}"));
            }
        }
    }
}
