using IF.Application.Abstractions;
using IF.Application.VaultService.Commands;
using IF.Application.VaultService.Queries;
using IF.Domain;
using IF.Domain.DTOs;
using IF.Domain.ErrorMessages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IF.BankingAPI.Controllers
{
    /// <summary>
    /// Admin API for Vault Management Operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "Banking API")]
    [ApiController]
    [Authorize]
    public class VaultManagementController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public VaultManagementController(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        /// <summary>
        /// Creates a new Vault
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateVaultAsync([FromBody] CreateVaultCommand command)
        {
            var result = await _commandDispatcher.DispatchAsync<CreateVaultCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
               success => Ok(),
               error => BadRequest(error.Message)
            );
        }

        /// <summary>
        /// Updates a Vault
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateVaultAsync([FromBody] UpdateVaultCommand command)
        {
            var result = await _commandDispatcher.DispatchAsync<UpdateVaultCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
               success => Ok(),
               error => BadRequest(error.Message)
            );
        }

        /// <summary>
        /// Deletes a Vault
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVaultAsync(Guid id)
        {
            var command = new DeleteVaultCommand(id);
            var result = await _commandDispatcher.DispatchAsync<DeleteVaultCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
               success => Ok(),
               error => BadRequest(error.Message)
            );
        }

        /// <summary>
        /// Gets a Vault by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVaultAsync(Guid id)
        {
            var query = new GetVaultByIdQuery(id);
            var result = await _commandDispatcher.QueryAsync<GetVaultByIdQuery, Result<VaultDTO, NotFoundError>>(query);

            return result.Match<IActionResult>(
               Ok,
               error => NotFound(error.Message)
           );
        }
    }
}
