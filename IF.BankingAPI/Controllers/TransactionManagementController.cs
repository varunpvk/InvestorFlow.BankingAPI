using IF.Application.Abstractions;
using IF.Application.TransactionService.Commands;
using IF.Application.TransactionService.Queries;
using IF.Domain;
using IF.Domain.DTOs;
using IF.Domain.ErrorMessages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IF.BankingAPI.Controllers
{
    /// <summary>
    /// Admin API for Transaction Operations
    /// Associates each Transaction with a CustomerAccount
    /// </summary>
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "Banking API")]
    [ApiController]
    [Authorize]
    public class TransactionManagementController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public TransactionManagementController(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        /// <summary>
        /// Creates a Transaction
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateTransactionAsync([FromBody] CreateTransactionCommand command)
        {
            var result = await _commandDispatcher.DispatchAsync<CreateTransactionCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
                              success => Ok(),
                              error => BadRequest(error.Message));
        }

        /// <summary>
        /// Gets a Transaction by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("transaction/{id}")]
        public async Task<IActionResult> GetTransactionByIdAsync(Guid id)
        {
            var query = new GetTransactionByIdQuery(id);
            var result = await _commandDispatcher.QueryAsync<GetTransactionByIdQuery, Result<TransactionDTO, NotFoundError>>(query);

            return result.Match<IActionResult>(
                              success => Ok(success),
                              error => BadRequest(error.Message));
        }

        /// <summary>
        /// Gets all Transactions associated with a customer account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet("account/{accountId}")]
        public async Task<IActionResult> GetTransactionsByAccountIdAsync(Guid accountId)
        {
            var query = new GetTransactionsByAccountIdQuery(accountId);
            var result = await _commandDispatcher.QueryAsync<GetTransactionsByAccountIdQuery, Result<IList<TransactionDTO>, NotFoundError>>(query);

            return result.Match<IActionResult>(
                              success => Ok(success),
                              error => BadRequest(error.Message));
        }

    }
}
