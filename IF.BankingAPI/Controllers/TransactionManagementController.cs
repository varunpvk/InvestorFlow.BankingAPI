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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionManagementController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public TransactionManagementController(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransactionAsync([FromBody] CreateTransactionCommand command)
        {
            var result = await _commandDispatcher.DispatchAsync<CreateTransactionCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
                              success => Ok(),
                              error => BadRequest(error.Message));
        }

        [HttpGet("transaction/{id}")]
        public async Task<IActionResult> GetTransactionByIdAsync(Guid id)
        {
            var query = new GetTransactionByIdQuery(id);
            var result = await _commandDispatcher.QueryAsync<GetTransactionByIdQuery, Result<TransactionDTO, NotFoundError>>(query);

            return result.Match<IActionResult>(
                              success => Ok(success),
                              error => BadRequest(error.Message));
        }

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
