using IF.Application.Abstractions;
using IF.Application.BankingService.Commands;
using IF.Domain;
using IF.Domain.Entities;
using IF.Domain.ErrorMessages;
using IF.Infrastructure.BankingRepository;

namespace IF.Application.BankingService.CommandHandlers
{
    public class CreateBankAccountCommandHandler : ICommandHandler<CreateBankAccountCommand, Result<bool, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateBankAccountCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool, ValidationError>> HandleAsync(CreateBankAccountCommand command)
        {
            try
            {
                //Begin transaction
                _unitOfWork.BeginTransaction();

                //Create a new Account
                var accountId = Guid.NewGuid();
                var account = new Account(accountId, command.Type);
                
                var accountSuccess = await _unitOfWork.Accounts.AddAsync(account);
                
                if (!accountSuccess)
                {
                    _unitOfWork.Rollback();
                    return Result<bool, ValidationError>.Failure(new ValidationError("Failed to create account"));
                }

                //Create a new Vault
                var vaultId = Guid.NewGuid();
                var vault = new Vault(vaultId, accountId);

                var vaultSuccess = await _unitOfWork.Vaults.AddAsync(vault);

                if (!vaultSuccess)
                {
                    _unitOfWork.Rollback();
                    return Result<bool, ValidationError>.Failure(new ValidationError("Failed to create vault"));
                }

                //Create a new User and associate the account to the user
                var customerAccountId = Guid.NewGuid();
                var userId = command.CustomerId;
                var customer = new CustomerAccount(customerAccountId, userId, accountId);

                var customerAccountSuccess = await _unitOfWork.CustomerAccounts.AddAsync(customer);

                if(!customerAccountSuccess)
                {
                    _unitOfWork.Rollback();
                    return Result<bool, ValidationError>.Failure(new ValidationError("Failed to create customer account"));
                }

                //commit the work
                _unitOfWork.Commit();
                return Result<bool, ValidationError>.Success(true);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result<bool, ValidationError>.Failure(new ValidationError($"Exception while creating bank account: {ex.Message}"));
            }
        }
    }
}
