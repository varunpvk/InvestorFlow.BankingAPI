using IF.Application.Abstractions;

namespace IF.Application.AccountService.Commands
{
    public class DeleteAccountCommand : ICommand
    {
        public Guid Id { get; private set; }

        public DeleteAccountCommand(Guid id)
        {
            Id = id;
        }
    }
}
