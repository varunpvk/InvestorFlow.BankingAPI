using Dapper;
using IF.Domain.DTOs;
using IF.Domain.Entities;
using IF.Infrastructure.BankingRepository;

namespace IF.Infrastructure
{
    public class VaultRepository : IVaultRepository
    {
        private readonly BankingContext _context;

        public VaultRepository(BankingContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Vault vault)
        {
            try
            {
                var query = "INSERT INTO Vault (Id, AccountID, CurrentBalance, Currency) VALUES (@Id, @AccountId, @CurrentBalance, @Currency)";
                var rowCount = await _context.Connection.ExecuteAsync(query, new
                {
                    Id = vault.Id,
                    AccountId = vault.AccountId,
                    CurrentBalance = vault.CurrentBalance.Amount,
                    Currency = vault.CurrentBalance.Currency
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
                var query = "DELETE FROM Vault WHERE Id = @Id";
                var rowCount = await _context.Connection.ExecuteAsync(query, new { Id = id });

                return rowCount == 1;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<VaultDTO> GetAsync(Guid id)
        {
            try
            {
                var query = @"SELECT * FROM Vault WHERE Id=@Id";
                var vault = await _context.Connection.QueryAsync<VaultDTO>(query, new
                {
                    Id = id
                });

                //Insert validation logic for empty sequence exception.

                return vault.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<VaultDTO> GetByAccountIdAsync(Guid accountId)
        {
            try
            {
                var query = @"SELECT * FROM Vault WHERE AccountId=@AccountId";
                var vault = await _context.Connection.QueryFirstOrDefaultAsync<VaultDTO>(query, new
                {
                    AccountId = accountId
                });

                return vault;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateAsync(Vault vault)
        {
            try
            {
                var query = "UPDATE Vault SET CurrentBalance = @CurrentBalance, Currency = @Currency WHERE Id = @id";
                var rowCount = await _context.Connection.ExecuteAsync(query, new
                {
                    CurrentBalance = vault.CurrentBalance.Amount,
                    Currency = vault.CurrentBalance.Currency,
                    id = vault.Id
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
