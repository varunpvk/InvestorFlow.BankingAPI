using IF.Application.Abstractions;

namespace IF.Application.VaultService.Commands
{
    public class DeleteVaultCommand : ICommand
    {
        public Guid Id { get; private set; }

        public DeleteVaultCommand(Guid id)
        {
            Id = id;
        }
    }
}
