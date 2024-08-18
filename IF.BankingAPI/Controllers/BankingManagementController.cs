using IF.Application.Abstractions;
using IF.Application.BankingService.Commands;
using IF.Application.BankingService.Queries;
using IF.Domain;
using IF.Domain.DTOs;
using IF.Domain.Enums;
using IF.Domain.ErrorMessages;
using Microsoft.AspNetCore.Mvc;

namespace IF.BankingAPI.Controllers
{
    /// <summary>
    /// Gateway for End User Banking Operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "InvestorFlow Banking")]
    [ApiController]
    public class BankingManagementController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ILogger<BankingManagementController> _logger;

        public BankingManagementController(
            ICommandDispatcher commandDispatcher, 
            ILogger<BankingManagementController> logger)
        {
            _commandDispatcher = commandDispatcher;
            _logger = logger;
        }

        /// <summary>
        /// Creates a Bank Account
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("create-account")]
        public async Task<IActionResult> CreateBankAccountAsync([FromBody] CreateBankAccountCommand command)
        {
            _logger.LogInformation("Creating Bank Account for Customer {CustomerId}", command.CustomerId);
            var result = await _commandDispatcher.DispatchAsync<CreateBankAccountCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
               success => Ok(),
               error => BadRequest(error.Message)
            );
        }

        /// <summary>
        /// Deletes a Bank Account
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns></returns>
        [HttpDelete("delete-account/{id}")]
        public async Task<IActionResult> DeleteBankAccountAsync(Guid id)
        {
            _logger.LogInformation("Deleting Bank Account {Id}", id);
            var command = new DeleteBankAccountCommand(id);
            var result = await _commandDispatcher.DispatchAsync<DeleteBankAccountCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
                success => Ok(),
                error => BadRequest(error.Message)
            );
        }

        /// <summary>
        /// Adds money to a Bank Account
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <param name="amount"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpPut("{id}/add-money/{amount}/type/{type}")]
        public async Task<IActionResult> AddMoneyToAccountAsync(Guid id, decimal amount, string type)
        {
            _logger.LogInformation("Adding {Amount} to Account {Id}", amount, id);
            var accountType = (AccountType) Enum.Parse(typeof(AccountType), type, true); 
            var command = new AddMoneyCommand(id, amount, "GBP", accountType);
            var result = await _commandDispatcher.DispatchAsync<AddMoneyCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
                 success => Ok(),
                 error => BadRequest(error.Message)
            );
        }

        /// <summary>
        /// Withdraws money from a Bank Account
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <param name="amount"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpPut("{id}/withdraw-money/{amount}/type/{type}")]
        public async Task<IActionResult> WithdrawMoneyFromAccountAsync(Guid id, decimal amount, string type)
        {
            _logger.LogInformation("Withdrawing {Amount} from Account {Id}", amount, id);
            var accountType = (AccountType)Enum.Parse(typeof(AccountType), type, true);
            var command = new WithdrawMoneyCommand(id, amount, "GBP", accountType);
            var result = await _commandDispatcher.DispatchAsync<WithdrawMoneyCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
                 success => Ok(),
                 error => BadRequest(error.Message)
            );
        }

        /// <summary>
        /// Transfers funds from one bank account to another
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <param name="accountId">Destination Account</param>
        /// <param name="amount"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpPut("{id}/transfer-to/{accountId}/amount/{amount}/type/{type}")]
        public async Task<IActionResult> TransferFundsToAccount(Guid id, Guid accountId, decimal amount, string type)
        {
            _logger.LogInformation("Transferring {Amount} from Account {Id} to Account {AccountId}", amount, id, accountId);
            var accountType = (AccountType)Enum.Parse(typeof(AccountType), type, true);
            var command = new TransferFundsCommand(id, accountType, accountId, amount, "GBP");
            var result = await _commandDispatcher.DispatchAsync<TransferFundsCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
                success => Ok(),
                error => BadRequest(error.Message)
            );
        }

        /// <summary>
        /// Gets the transaction history for a bank account
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns></returns>
        [HttpGet("tranasctions/{id}")]
        public async Task<IActionResult> GetTransactionHistoryAsync(Guid id)
        {
            _logger.LogInformation("Getting Transaction History for Account {Id}", id);
            var query = new GetTransactionHistoryQuery(id);
            var result = await _commandDispatcher.QueryAsync<GetTransactionHistoryQuery, Result<IDictionary<Guid, IList<TransactionDTO>>, NotFoundError>>(query);

            return result.Match<IActionResult>(
                Ok,
                error => BadRequest(error.Message)
            );
        }
    }
}
