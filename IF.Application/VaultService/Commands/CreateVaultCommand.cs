using IF.Application.Abstractions;
using IF.Domain.Entities;

namespace IF.Application.VaultService.Commands
{
    public class CreateVaultCommand : ICommand
    {
        public Guid Id { get; private set; }
        public Guid AccountId { get; private set; }

        public CreateVaultCommand(Guid id, Guid accountId)
        {
            Id = id;
            AccountId = accountId;
        }
    }
}
