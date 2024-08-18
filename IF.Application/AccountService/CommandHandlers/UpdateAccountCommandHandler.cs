using IF.Application.Abstractions;
using IF.Application.AccountService.Commands;
using IF.Domain;
using IF.Domain.Entities;
using IF.Domain.ErrorMessages;
using IF.Infrastructure.BankingRepository;

namespace IF.Application.AccountService.CommandHandlers
{
    public class UpdateAccountCommandHandler : ICommandHandler<UpdateAccountCommand, Result<bool, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAccountCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool, ValidationError>> HandleAsync(UpdateAccountCommand command)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                var account = await _unitOfWork.Accounts.GetAsync(command.Id);

                if (account != null)
                {
                    var updateAccount = new Account(Guid.Parse(account.Id), command.Type);
                    var success = await _unitOfWork.Accounts.UpdateAsync(updateAccount);

                    if (success)
                    {
                        _unitOfWork.Commit();
                        return Result<bool, ValidationError>.Success(true);
                    }
                }

                _unitOfWork.Rollback();
                return Result<bool, ValidationError>.Failure(new ValidationError("Failed to update account"));
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result<bool, ValidationError>.Failure(new ValidationError($"Exception while updating account: {ex.Message}"));
            }
           
        }
    }
}
