using IF.Application.Abstractions;
using IF.Application.BankingService.Commands;
using IF.Domain;
using IF.Domain.Entities;
using IF.Domain.ErrorMessages;
using IF.Infrastructure.BankingRepository;
using Microsoft.Extensions.Logging;

namespace IF.Application.BankingService.CommandHandlers
{
    public class CreateBankAccountCommandHandler : ICommandHandler<CreateBankAccountCommand, Result<bool, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateBankAccountCommandHandler> _logger;

        public CreateBankAccountCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateBankAccountCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
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
                    _logger.LogError("Failed to create account");
                    _unitOfWork.Rollback();
                    return Result<bool, ValidationError>.Failure(new ValidationError("Failed to create account"));
                }

                //Create a new Vault
                var vaultId = Guid.NewGuid();
                var vault = new Vault(vaultId, accountId);

                var vaultSuccess = await _unitOfWork.Vaults.AddAsync(vault);

                if (!vaultSuccess)
                {
                    _logger.LogError("Failed to create vault");
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
                    _logger.LogError("Failed to create customer account");
                    _unitOfWork.Rollback();
                    return Result<bool, ValidationError>.Failure(new ValidationError("Failed to create customer account"));
                }

                //commit the work
                _logger.LogInformation("Bank Account created successfully");
                _unitOfWork.Commit();
                return Result<bool, ValidationError>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while creating bank account");
                _unitOfWork.Rollback();
                return Result<bool, ValidationError>.Failure(new ValidationError($"Exception while creating bank account: {ex.Message}"));
            }
        }
    }
}
