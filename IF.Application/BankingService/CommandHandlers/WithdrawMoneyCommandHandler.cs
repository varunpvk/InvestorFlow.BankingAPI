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
    public class WithdrawMoneyCommandHandler : ICommandHandler<WithdrawMoneyCommand, Result<bool, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WithdrawMoneyCommandHandler> _logger;

        public WithdrawMoneyCommandHandler(IUnitOfWork unitOfWork, ILogger<WithdrawMoneyCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<bool, ValidationError>> HandleAsync(WithdrawMoneyCommand command)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                // Get Account
                var customerAccounts = await _unitOfWork.CustomerAccounts.GetByCustomerIdAsync(command.CustomerId);

                foreach (var customerAccount in customerAccounts)
                {
                    var account = await _unitOfWork.Accounts.GetAsync(Guid.Parse(customerAccount.AccountId));
                    if (account != null)
                    {
                        // Get Vault
                        var vaultDTO = await _unitOfWork.Vaults.GetByAccountIdAsync(Guid.Parse(account.Id));
                        vaultDTO.CurrentBalance -= command.Amount;
                        var vault = new Vault(Guid.Parse(vaultDTO.Id), Guid.Parse(vaultDTO.AccountId));

                        // Withdraw Money
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
                            new TransactionDetails(TransactionType.Withdrawal, command.Amount, command.Currency),
                            DateTime.UtcNow);
                        var transactionSuccess = await _unitOfWork.Transactions.AddAsync(transaction);

                        if (!transactionSuccess)
                        {
                            _logger.LogError("Failed to add transaction");
                            _unitOfWork.Rollback();
                            return Result<bool, ValidationError>.Failure(new ValidationError("Failed to add transaction"));
                        }

                        _logger.LogInformation("Money withdrawn successfully");
                        _unitOfWork.Commit();
                        return Result<bool, ValidationError>.Success(true);
                    }
                }

                _logger.LogError("Failed to withdraw money");
                _unitOfWork.Rollback();
                return Result<bool, ValidationError>.Failure(new ValidationError("Failed to withdraw money"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while withdrawing money");
                _unitOfWork.Rollback();
                return Result<bool, ValidationError>.Failure(new ValidationError($"Exception while withdrawing money: {ex.Message}"));
            }
        }
    }
}
