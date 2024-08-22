using IF.Application.Abstractions;
using IF.Application.BankingService.Commands;
using IF.Application.BankingService.Queries;
using IF.Domain;
using IF.Domain.DTOs;
using IF.Domain.ErrorMessages;
using Microsoft.AspNetCore.Mvc;

namespace IF.BankingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "InvestorFlow Banking")]
    [ApiController]
    public class BankingController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ILogger<BankingController> _logger;

        public BankingController(
            ICommandDispatcher commandDispatcher, 
            ILogger<BankingController> logger)
        {
            _commandDispatcher = commandDispatcher;
            _logger = logger;
        }

        [HttpPost("create-customer-account")]
        public async Task<IActionResult> CreateBankAccountAsync([FromBody] CreateBankAccountCommand command)
        {
            _logger.LogInformation("Creating Bank Account for Customer {CustomerId}", command.CustomerId);
            var result = await _commandDispatcher.DispatchAsync<CreateBankAccountCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
               success => Ok(),
               error => BadRequest(error.Message)
            );
        }

        [HttpDelete("delete-customer-account")]
        public async Task<IActionResult> DeleteBankAccountAsync([FromBody] DeleteBankAccountCommand command)
        {
            _logger.LogInformation("Deleting Bank Account {Id}", command.CustomerId);
            var result = await _commandDispatcher.DispatchAsync<DeleteBankAccountCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
                success => Ok(),
                error => BadRequest(error.Message)
            );
        }

        [HttpPut("add-money")]
        public async Task<IActionResult> AddMoneyToAccountAsync([FromBody] AddMoneyCommand command)
        {
            _logger.LogInformation("Adding {Amount} to Account {Id}", command.Amount, command.CustomerId);
            var result = await _commandDispatcher.DispatchAsync<AddMoneyCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
                 success => Ok(),
                 error => BadRequest(error.Message)
            );
        }

        [HttpPut("withdraw-money")]
        public async Task<IActionResult> WithdrawMoneyFromAccountAsync([FromBody] WithdrawMoneyCommand command)
        {
            _logger.LogInformation("Withdrawing {Amount} from Account {Id}", command.Amount, command.CustomerId);
            var result = await _commandDispatcher.DispatchAsync<WithdrawMoneyCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
                 success => Ok(),
                 error => BadRequest(error.Message)
            );
        }

        [HttpPut("transfer-funds")]
        public async Task<IActionResult> TransferFundsToAccount([FromBody] TransferFundsCommand command)
        {
            _logger.LogInformation("Transferring {Amount} from Account {Id} to Account {AccountId}", command.Amount, command.CustomerId, command.DestinationAccountId);
            var result = await _commandDispatcher.DispatchAsync<TransferFundsCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
                success => Ok(),
                error => BadRequest(error.Message)
            );
        }

        [HttpGet("transaction-history")]
        public async Task<IActionResult> GetTransactionHistoryAsync(Guid customerId)
        {
            _logger.LogInformation("Getting Transaction History for Account {Id}", customerId);
            var query = new GetTransactionHistoryQuery(customerId);
            var result = await _commandDispatcher.QueryAsync<GetTransactionHistoryQuery, Result<IDictionary<Guid, IList<TransactionDTO>>, NotFoundError>>(query);

            return result.Match<IActionResult>(
                Ok,
                error => BadRequest(error.Message)
            );
        }
    }
}
