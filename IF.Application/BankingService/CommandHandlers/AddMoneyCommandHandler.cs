using IF.Application.Abstractions;
using IF.Application.BankingService.Commands;
using IF.Domain;
using IF.Domain.Entities;
using IF.Domain.Enums;
using IF.Domain.ErrorMessages;
using IF.Domain.ValueObjects;
using IF.Infrastructure.BankingRepository;
using Microsoft.Extensions.Logging;

namespace IF.Application.BankingService.CommandHandlers
{
    public class AddMoneyCommandHandler : ICommandHandler<AddMoneyCommand, Result<bool, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddMoneyCommandHandler> _logger;

        public AddMoneyCommandHandler(IUnitOfWork unitOfWork, ILogger<AddMoneyCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<bool, ValidationError>> HandleAsync(AddMoneyCommand command)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                // Get Account
                var customerAccounts = await _unitOfWork.CustomerAccounts.GetByCustomerIdAsync(command.CustomerId);

                foreach (var customerAccount in customerAccounts)
                {
                    var account = await _unitOfWork.Accounts.GetAsync(Guid.Parse(customerAccount.AccountId));
                    if(account != null)
                    {
                        // Get Vault
                        var vaultDTO = await _unitOfWork.Vaults.GetByAccountIdAsync(Guid.Parse(account.Id));
                        vaultDTO.CurrentBalance += command.Amount;
                        var vault = new Vault(Guid.Parse(vaultDTO.Id), Guid.Parse(vaultDTO.AccountId));
                        
                        // Add Money
                        vault.UpdateVault(vaultDTO.CurrentBalance, command.Currency);
                        var success = await _unitOfWork.Vaults.UpdateAsync(vault);

                        if (!success)
                        {
                            _logger.LogError("Failed to update vault");
                            _unitOfWork.Rollback();
                            return Result<bool, ValidationError>.Failure(new ValidationError("Failed to update vault"));
                        }

                        // Add Transaction
                        var transaction = new Transaction(
                            Guid.NewGuid(), 
                            Guid.Parse(customerAccount.Id), 
                            new TransactionDetails(TransactionType.Deposit, command.Amount, command.Currency),
                            DateTime.UtcNow);
                        var transactionSuccess = await _unitOfWork.Transactions.AddAsync(transaction);

                        if (!transactionSuccess)
                        {
                            _logger.LogError("Failed to add transaction");
                            _unitOfWork.Rollback();
                            return Result<bool, ValidationError>.Failure(new ValidationError("Failed to add transaction"));
                        }

                        _logger.LogInformation("Money added successfully");
                        _unitOfWork.Commit();
                        return Result<bool, ValidationError>.Success(true);
                    }
                }

                _logger.LogError("Failed to add money");
                _unitOfWork.Rollback();
                return Result<bool, ValidationError>.Failure(new ValidationError("Failed to add money"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while adding money");
                _unitOfWork.Rollback();
                return Result<bool, ValidationError>.Failure(new ValidationError($"Exception while adding money: {ex.Message}"));
            }
        }
    }
}
