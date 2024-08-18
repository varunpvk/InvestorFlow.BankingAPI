using IF.Application.Abstractions;
using IF.Application.AccountService.Commands;
using IF.Domain;
using IF.Domain.Entities;
using IF.Domain.ErrorMessages;
using IF.Infrastructure;
using IF.Infrastructure.BankingRepository;

namespace IF.Application.AccountService.CommandHandlers
{
    public class CreateAccountCommandHandler : ICommandHandler<CreateAccountCommand, Result<bool, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateAccountCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool, ValidationError>> HandleAsync(CreateAccountCommand command)
        {
            try
            {
                var account = new Account(command.Id,command.Type);

                _unitOfWork.BeginTransaction();
                var success = await _unitOfWork.Accounts.AddAsync(account);
                if (success)
                {
                    _unitOfWork.Commit();
                    return Result<bool, ValidationError>.Success(true);  
                }
                _unitOfWork.Rollback();
                return Result<bool, ValidationError>.Failure(new ValidationError("Failed to create account"));
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result<bool, ValidationError>.Failure(new ValidationError($"Exception while creating account: {ex.Message}"));
            }
        }
    }
}
