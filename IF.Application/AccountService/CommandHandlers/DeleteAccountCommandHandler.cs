using IF.Application.Abstractions;
using IF.Application.AccountService.Commands;
using IF.Domain;
using IF.Domain.ErrorMessages;
using IF.Infrastructure.BankingRepository;

namespace IF.Application.AccountService.CommandHandlers
{
    public class DeleteAccountCommandHandler : ICommandHandler<DeleteAccountCommand, Result<bool, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAccountCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool, ValidationError>> HandleAsync(DeleteAccountCommand command)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                var success = await _unitOfWork.Accounts.DeleteAsync(command.Id);

                if(success)
                {
                    _unitOfWork.Commit();
                    return Result<bool, ValidationError>.Success(true);
                }

                _unitOfWork.Rollback();
                return Result<bool, ValidationError>.Failure(new ValidationError("Failed to delete account"));
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result<bool, ValidationError>.Failure(new ValidationError($"Exception while deleting account: {ex.Message}"));
            }
            
        }
    }
}
