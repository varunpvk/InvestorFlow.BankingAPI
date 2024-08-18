using IF.Application.Abstractions;
using IF.Application.AccountService.Commands;
using IF.Application.AccountService.Queries;
using IF.Domain;
using IF.Domain.DTOs;
using IF.Domain.ErrorMessages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IF.BankingAPI.Controllers
{
    /// <summary>
    /// Admin API for Banking Admin CRUD Operations
    /// </summary>
    [Route("api")]
    [ApiExplorerSettings(GroupName = "Banking API")]
    [ApiController]
    [Authorize]
    public class AccountManagementController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public AccountManagementController(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        /// <summary>
        /// Creates a new Account
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateAccountAsync([FromBody] CreateAccountCommand command)
        {
            var result = await _commandDispatcher.DispatchAsync<CreateAccountCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
               success => Ok(),
               error => BadRequest(error.Message)
            );
        }

        /// <summary>
        /// Deletes an Account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccountAsync(Guid id)
        {
            var command = new DeleteAccountCommand(id);
            var result = await _commandDispatcher.DispatchAsync<DeleteAccountCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
               success => Ok(),
               error => BadRequest(error.Message)
            );
        }

        /// <summary>
        /// Gets an Account by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountAsync(Guid id)
        {
            var query = new GetAccountByIdQuery(id);
            var result = await _commandDispatcher.QueryAsync<GetAccountByIdQuery, Result<AccountDTO, NotFoundError>>(query);

            return result.Match<IActionResult>(
               Ok,
               error => NotFound(error.Message)
           );
        }
    }
}
