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
    public class TransferFundsCommandHandler : ICommandHandler<TransferFundsCommand, Result<bool, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransferFundsCommandHandler> _logger;

        public TransferFundsCommandHandler(IUnitOfWork unitOfWork, ILogger<TransferFundsCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<bool, ValidationError>> HandleAsync(TransferFundsCommand command)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                // get the account associated to the user.
                var customerAccounts = await _unitOfWork.CustomerAccounts.GetByCustomerIdAsync(command.CustomerId);

                foreach (var customerAccount in customerAccounts)
                {
                    var account = await _unitOfWork.Accounts.GetAsync(Guid.Parse(customerAccount.AccountId));
                    if (account != null)
                    {
                        // get the vault associated to the source and destiantion account.
                        // vault - Source Account
                        var sourceVaultDTO = await _unitOfWork.Vaults.GetByAccountIdAsync(Guid.Parse(account.Id));
                        sourceVaultDTO.CurrentBalance -= command.Amount;
                        if (sourceVaultDTO != null)
                        {
                            // update vault for source account.
                            var vault = new Vault(Guid.Parse(sourceVaultDTO.Id), Guid.Parse(sourceVaultDTO.AccountId));
                            vault.UpdateVault(sourceVaultDTO.CurrentBalance, sourceVaultDTO.Currency);
                            var success = await _unitOfWork.Vaults.UpdateAsync(vault);
                            if (!success)
                            {
                                _logger.LogError("Failed to transfer funds");
                                _unitOfWork.Rollback();
                                return Result<bool, ValidationError>.Failure(new ValidationError("Failed to transfer funds"));
                            }

                            //create a transaction for source account.
                            var transaction = new Transaction(
                            Guid.NewGuid(),
                            Guid.Parse(customerAccount.Id),
                            new TransactionDetails(
                                TransactionType.TransferSent, 
                                command.Amount, 
                                command.Currency),
                            DateTime.UtcNow);
                            var transactionSuccess = await _unitOfWork.Transactions.AddAsync(transaction);

                            if (!transactionSuccess)
                            {
                                _logger.LogError("Failed to add transaction");
                                _unitOfWork.Rollback();
                                return Result<bool, ValidationError>.Failure(new ValidationError("Failed to add transaction"));
                            }
                        }

                        // vault - Destination Account
                        var destinationVaultDTO = await _unitOfWork.Vaults.GetByAccountIdAsync(command.DestinationAccountId);
                        destinationVaultDTO.CurrentBalance += command.Amount;
                        if (destinationVaultDTO != null)
                        {
                            // update vault for source account.
                            var vault = new Vault(Guid.Parse(destinationVaultDTO.Id), Guid.Parse(destinationVaultDTO.AccountId));
                            vault.UpdateVault(destinationVaultDTO.CurrentBalance, destinationVaultDTO.Currency);
                            var success = await _unitOfWork.Vaults.UpdateAsync(vault);
                            if (!success)
                            {
                                _logger.LogError("Failed to transfer funds");
                                _unitOfWork.Rollback();
                                return Result<bool, ValidationError>.Failure(new ValidationError("Failed to transfer funds"));
                            }

                            //create a transaction for source account.
                            var destiantioncustomerAccount = await _unitOfWork.CustomerAccounts.GetByAccountIdAsync(command.DestinationAccountId);
                            var transaction = new Transaction(
                            Guid.NewGuid(),
                            Guid.Parse(destiantioncustomerAccount.Id),
                            new TransactionDetails(
                                TransactionType.TransaferReceived,
                                command.Amount,
                                command.Currency),
                            DateTime.UtcNow);
                            var transactionSuccess = await _unitOfWork.Transactions.AddAsync(transaction);

                            if (!transactionSuccess)
                            {
                                _logger.LogError("Failed to add transaction");
                                _unitOfWork.Rollback();
                                return Result<bool, ValidationError>.Failure(new ValidationError("Failed to add transaction"));
                            }
                        }

                        _logger.LogInformation("Funds transferred successfully");
                        _unitOfWork.Commit();
                        return Result<bool, ValidationError>.Success(true);
                    }
                }

                _logger.LogError("Failed to transfer funds");
                _unitOfWork.Rollback();
                return Result<bool, ValidationError>.Failure(new ValidationError("Failed to transfer funds"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while transferring funds");
                _unitOfWork.Rollback();
                return Result<bool, ValidationError>.Failure(new ValidationError($"Exception while transferring funds: {ex.Message}"));
            }
        }
    }
}
