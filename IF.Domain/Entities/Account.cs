using IF.Domain.Abstractions;
using IF.Domain.Enums;

namespace IF.Domain.Entities
{
    public sealed class Account : Entity
    {
        public Account(Guid id, AccountType type)
            : base(id)
        {
            Type = type;
            SortCode = GenerateSortCode();
            AccountNumber = GenerateAccountNumber();
        }

        public Account()
            : base(Guid.NewGuid())
        {
        }

        public AccountType Type { get; private set; }
        public string SortCode { get; private set; }
        public string AccountNumber { get; private set; }

        private string GenerateSortCode()
        {
            string sortCode = "";

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    sortCode += new Random().Next(0, 9).ToString();
                }
                if (i < 2)
                    sortCode += "-";
            }

            return sortCode;
        }

        private string GenerateAccountNumber()
        {
            string accountNumber = "";

            for (int i = 0; i < 8; i++)
            {
                accountNumber += new Random().Next(0, 9).ToString();
            }

            return accountNumber;
        }
    }
}
