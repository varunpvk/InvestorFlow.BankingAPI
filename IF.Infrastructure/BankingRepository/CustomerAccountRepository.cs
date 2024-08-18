using Dapper;
using IF.Domain.DTOs;
using IF.Domain.Entities;

namespace IF.Infrastructure.BankingRepository
{
    public class CustomerAccountRepository : ICustomerAccountRepository
    {
        private readonly BankingContext _context;
        public CustomerAccountRepository(BankingContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(CustomerAccount customer)
        {
            try
            {
                var query = "INSERT into CustomerAccount (Id, CustomerId, AccountId) Values (@Id, @CustomerId, @AccountId)";
                var rowCount = await _context.Connection.ExecuteAsync(query, new
                {
                    Id = customer.Id,
                    CustomerId = customer.CustomerId,
                    AccountId = customer.AccountId
                });

                return rowCount == 1;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var query = @"DELETE FROM CustomerAccount where Id = @Id";
                var rowCount = await _context.Connection.ExecuteAsync(query, new { Id = id });

                return rowCount == 1;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CustomerAccountDTO> GetByCustomerAccountIdAsync(Guid customerAccountId)
        {
            try
            {
                var query = @"SELECT * FROM CustomerAccount WHERE Id = @CustomerAccountId";
                var customerAccount = await _context.Connection.QueryFirstOrDefaultAsync<CustomerAccountDTO>(query, new { CustomerAccountId = customerAccountId });

                return customerAccount;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<CustomerAccountDTO>> GetByCustomerIdAsync(Guid customerId)
        {
            try
            {
                var query = @"SELECT * FROM CustomerAccount WHERE CustomerId = @CustomerId";
                var customerAccounts = await _context.Connection.QueryAsync<CustomerAccountDTO>(query, new { CustomerId = customerId });

                return customerAccounts;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CustomerAccountDTO> GetByAccountIdAsync(Guid accountId)
        {
            try
            {
                var query = @"SELECT * FROM CustomerAccount WHERE AccountId = @AccountId";
                var customerAccount = await _context.Connection.QueryFirstOrDefaultAsync<CustomerAccountDTO>(query, new { AccountId = accountId });

                return customerAccount;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
