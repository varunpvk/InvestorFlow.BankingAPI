namespace IF.Domain.DTOs
{
    public class AccountDTO
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string SortCode { get; private set; }
        public string AccountNumber { get; private set; }

        public AccountDTO(string id, string name, string sortCode, string accountNumber)
        {
            Id = id;
            Name = name;
            SortCode = sortCode;
            AccountNumber = accountNumber;
        }

        public AccountDTO()
        {
            
        }
    }
}
