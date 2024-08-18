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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VaultManagementController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public VaultManagementController(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        [HttpPost]
        public async Task<IActionResult> CreateVaultAsync([FromBody] CreateVaultCommand command)
        {
            var result = await _commandDispatcher.DispatchAsync<CreateVaultCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
               success => Ok(),
               error => BadRequest(error.Message)
            );
        }

        [HttpPut]
        public async Task<IActionResult> UpdateVaultAsync([FromBody] UpdateVaultCommand command)
        {
            var result = await _commandDispatcher.DispatchAsync<UpdateVaultCommand, Result<bool, ValidationError>>(command);

            return result.Match<IActionResult>(
               success => Ok(),
               error => BadRequest(error.Message)
            );
        }

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
