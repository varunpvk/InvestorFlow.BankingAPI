using IF.Application.Abstractions;
using IF.Domain.Enums;

namespace IF.Application.AccountService.Commands
{
    public class UpdateAccountCommand : ICommand
    {
        public Guid Id { get; private set; }
        public AccountType Type { get; private set; }
        public string SortCode { get; private set; }
        public string AccountNo { get; private set; }

        public UpdateAccountCommand(
            Guid id, 
            AccountType type,
            string sortCode,
            string accountNo)
        {
            Id = id;
            Type = type;
            SortCode = sortCode;
            AccountNo = accountNo;
        }
    }
}
