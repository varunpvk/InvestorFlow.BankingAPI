using IF.Application.Abstractions;
using IF.Application.BankingService.Commands;
using IF.Domain;
using IF.Domain.ErrorMessages;
using IF.Infrastructure.BankingRepository;
using Microsoft.Extensions.Logging;

namespace IF.Application.BankingService.CommandHandlers
{
    public class DeleteBankAccountCommandHandler : ICommandHandler<DeleteBankAccountCommand, Result<bool, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteBankAccountCommandHandler> _logger;

        public DeleteBankAccountCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteBankAccountCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<bool, ValidationError>> HandleAsync(DeleteBankAccountCommand command)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                //Get Customer Account Details
                var customerAccountDetails = await _unitOfWork.CustomerAccounts.GetByCustomerIdAsync(command.CustomerId);

                foreach (var customerAccount in customerAccountDetails)
                {
                    //Delete Transactions
                    var transactions = await _unitOfWork.Transactions.GetByAccountIdAsync(Guid.Parse(customerAccount.Id));

                    foreach (var transaction in transactions)
                    {
                        var deleteSuccess = await _unitOfWork.Transactions.DeleteAsync(Guid.Parse(transaction.Id));

                        if(!deleteSuccess)
                        {
                            _logger.LogError("Failed to delete transaction");
                            _unitOfWork.Rollback();
                            return Result<bool, ValidationError>.Failure(new ValidationError("Failed to delete transaction"));
                        }
                    }

                    //Delete Vault
                    var account = await _unitOfWork.Accounts.GetAsync(Guid.Parse(customerAccount.AccountId));
                    var vault = await _unitOfWork.Vaults.GetByAccountIdAsync(Guid.Parse(account.Id));
                    var success = await _unitOfWork.Vaults.DeleteAsync(Guid.Parse(vault.Id));

                    if (!success)
                    {
                        _logger.LogError("Failed to delete vault");
                        _unitOfWork.Rollback();
                        return Result<bool, ValidationError>.Failure(new ValidationError("Failed to delete vault"));
                    }

                    //Delete Account
                    success = await _unitOfWork.Accounts.DeleteAsync(Guid.Parse(account.Id));

                    if (!success)
                    {
                        _logger.LogError("Failed to delete account");
                        _unitOfWork.Rollback();
                        return Result<bool, ValidationError>.Failure(new ValidationError("Failed to delete account"));
                    }

                    //Delete Customer Account
                    success = await _unitOfWork.CustomerAccounts.DeleteAsync(Guid.Parse(customerAccount.Id));

                    if (!success)
                    {
                        _logger.LogError("Failed to delete customer account");
                        _unitOfWork.Rollback();
                        return Result<bool, ValidationError>.Failure(new ValidationError("Failed to delete customer account"));
                    }
                }

                _logger.LogInformation("Bank account deleted successfully");
                _unitOfWork.Commit();
                return Result<bool, ValidationError>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while deleting bank account");
                _unitOfWork.Rollback();
                return Result<bool, ValidationError>.Failure(new ValidationError($"Exception while deleting bank account: {ex.Message}"));
            }
        }
    }
}
