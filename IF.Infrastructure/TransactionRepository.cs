using Dapper;
using IF.Domain.DTOs;
using IF.Domain.Entities;
using IF.Infrastructure.BankingRepository;

namespace IF.Infrastructure
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly BankingContext _context;

        public TransactionRepository(BankingContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Transaction transaction)
        {
            try
            {
                var query = "INSERT INTO TransactionHistory (Id, CustomerAccountId, Type, Amount, Currency, Description, TransactionDate) VALUES (@Id, @CustomerAccountId, @Type, @Amount, @Currency, @Description, @TransactionDate)";
                var rowCount = await _context.Connection.ExecuteAsync(query, new
                {
                    Id = transaction.Id,
                    CustomerAccountId = transaction.CustomerAccountId,
                    Type = transaction.Details.Type,
                    Amount = transaction.Details.Amount,
                    Currency = transaction.Details.Currency,
                    Description = transaction.Details.Description,
                    TransactionDate = transaction.TransactionDateTimeUtc
                });

                return rowCount == 1;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid Id)
        {
            try
            {
                var query = "DELETE FROM TransactionHistory where Id=@Id";
                var result = await _context.Connection.ExecuteAsync(query, new { Id });

                return result == 1;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<TransactionDTO>> GetByAccountIdAsync(Guid accountId)
        {
            try
            {
                var query = "SELECT * FROM TransactionHistory where CustomerAccountId = @accountId";
                var result = await _context.Connection.QueryAsync<TransactionDTO>(query, new { accountId });

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<TransactionDTO> GetByIdAsync(Guid id)
        {
            try
            {
                var query = "SELECT * FROM TransactionHistory where Id = @Id";
                var result = _context.Connection.QueryFirstOrDefaultAsync<TransactionDTO>(query, new { Id = id });

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
