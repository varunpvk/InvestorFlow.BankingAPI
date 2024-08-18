using IF.Application.Abstractions;
using IF.Domain.Enums;

namespace IF.Application.AccountService.Commands
{
    public class CreateAccountCommand : ICommand
    {
        public Guid Id { get; private set; }
        public AccountType Type { get; private set; }

        public CreateAccountCommand(
            Guid id,
            AccountType type
            )
        {
            Id = id;
            Type = type;
        }
    }
}
