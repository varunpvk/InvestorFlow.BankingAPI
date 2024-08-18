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
    [Route("api/[controller]")]
    [ApiController]
    public class BankingManagementController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public BankingManagementController(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        [HttpPost("create-account")]
        public async Task<IActionResult> CreateBankAccountAsync([FromBody] CreateBankAccountCommand command)
        {
            var result = await _commandDispatcher.DispatchAsync<CreateBankAccountCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
               success => Ok(),
               error => BadRequest(error.Message)
            );
        }

        [HttpDelete("delete-account/{id}")]
        public async Task<IActionResult> DeleteBankAccountAsync(Guid id)
        {
            var command = new DeleteBankAccountCommand(id);
            var result = await _commandDispatcher.DispatchAsync<DeleteBankAccountCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
                success => Ok(),
                error => BadRequest(error.Message)
            );
        }

        [HttpPut("{id}/add-money/{amount}/type/{type}")]
        public async Task<IActionResult> AddMoneyToAccountAsync(Guid id, decimal amount, string type)
        {
            var accountType = (AccountType) Enum.Parse(typeof(AccountType), type, true); 
            var command = new AddMoneyCommand(id, amount, "GBP", accountType);
            var result = await _commandDispatcher.DispatchAsync<AddMoneyCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
                 success => Ok(),
                 error => BadRequest(error.Message)
            );
        }

        [HttpPut("{id}/withdraw-money/{amount}/type/{type}")]
        public async Task<IActionResult> WithdrawMoneyFromAccountAsync(Guid id, decimal amount, string type)
        {
            var accountType = (AccountType)Enum.Parse(typeof(AccountType), type, true);
            var command = new WithdrawMoneyCommand(id, amount, "GBP", accountType);
            var result = await _commandDispatcher.DispatchAsync<WithdrawMoneyCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
                 success => Ok(),
                 error => BadRequest(error.Message)
            );
        }

        [HttpPut("{id}/transfer-to/{accountId}/amount/{amount}/type/{type}")]
        public async Task<IActionResult> TransferFundsToAccount(Guid id, Guid accountId, decimal amount, string type)
        {
            var accountType = (AccountType)Enum.Parse(typeof(AccountType), type, true);
            var command = new TransferFundsCommand(id, accountType, accountId, amount, "GBP");
            var result = await _commandDispatcher.DispatchAsync<TransferFundsCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
                success => Ok(),
                error => BadRequest(error.Message)
            );
        }

        [HttpGet("tranasctions/{id}")]
        public async Task<IActionResult> GetTransactionHistoryAsync(Guid id)
        {
            var query = new GetTransactionHistoryQuery(id);
            var result = await _commandDispatcher.QueryAsync<GetTransactionHistoryQuery, Result<IDictionary<Guid, IList<TransactionDTO>>, NotFoundError>>(query);

            return result.Match<IActionResult>(
                Ok,
                error => BadRequest(error.Message)
            );
        }
    }
}
