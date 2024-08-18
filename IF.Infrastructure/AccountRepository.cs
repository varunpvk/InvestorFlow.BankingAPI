using Dapper;
using IF.Domain.DTOs;
using IF.Domain.Entities;
using IF.Infrastructure.BankingRepository;

namespace IF.Infrastructure
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankingContext _context;

        public AccountRepository(BankingContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Account account)
        {
            try
            {
                var query = "INSERT INTO Account (Id, Name, SortCode, AccountNumber) VALUES (@Id, @Name, @SortCode, @AccountNumber)";
                var rowCount = await _context.Connection.ExecuteAsync(query, new
                {
                    Id = account.Id,
                    Name = account.Type,
                    SortCode = account.SortCode,
                    AccountNumber = account.AccountNumber
                });

                return rowCount == 1;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var query = "DELETE FROM Account WHERE Id = @Id";
                var rowCount = await _context.Connection.ExecuteAsync(query, new { Id = id });

                return rowCount == 1;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<AccountDTO> GetAsync(Guid id)
        {
            try
            {
                var query = @"SELECT * FROM Account WHERE Id=@Id";
                var accounts = await _context.Connection.QueryAsync<AccountDTO>(query, new
                {
                    Id=id
                });

                //Insert validation logic for empty sequence exception.

                return accounts.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateAsync(Account account)
        {
            try
            {
                var query = "UPDATE Account SET Name = @Name, SortCode = @SortCode, AccountNumber = @AccountNumber WHERE Id = @id";
                var rowCount = await _context.Connection.ExecuteAsync(query, new
                {
                    Name = account.Type,
                    SortCode = account.SortCode,
                    AccountNumber = account.AccountNumber,
                    id = account.Id
                });

                return rowCount == 1;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
