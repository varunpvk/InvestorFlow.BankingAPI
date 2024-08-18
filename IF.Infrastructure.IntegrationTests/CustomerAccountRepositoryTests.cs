using Dapper;
using FluentAssertions;
using IF.Domain.Entities;
using IF.Infrastructure.BankingRepository;
using IF.Infrastructure.IntegrationTests.Fixtures;
using IF.Infrastructure.IntegrationTests.Helper;

namespace IF.Infrastructure.IntegrationTests
{
    [Collection("SequentialTests")]
    public class CustomerAccountRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly BankingContext _context;

        public CustomerAccountRepositoryTests(DatabaseFixture databaseFixture)
        {
            _context = databaseFixture.TestHelper.Context;
        }

        public async Task DisposeAsync()
        {
            CleanDatabase().Wait();
        }

        public Task InitializeAsync()
        {
            SeedData();
            return Task.CompletedTask;
        }

        private Task CleanDatabase()
        {
            _context.Connection.Execute("DELETE FROM Vault;");
            _context.Connection.Execute("DELETE FROM Account;");
            _context.Connection.Execute("DELETE FROM TransactionHistory;");
            _context.Connection.Execute("DELETE FROM CustomerAccount;");
            return Task.CompletedTask;
        }

        private Task SeedData()
        {
            var accountSeed = SqliteTestHelper.accountSeed;
            var vaultSeed = SqliteTestHelper.vaultSeed;
            var customerAccountSeed = SqliteTestHelper.customerAccountSeed; 
            
            var seedAccountDataQuery = @"INSERT INTO Account (Id, Name, SortCode, AccountNumber) VALUES (@Id, @Name, @SortCode, @AccountNumber);";
            _context.Connection.Execute(seedAccountDataQuery, new
            {
                Id = accountSeed.Id,
                Name = accountSeed.Type,
                SortCode = accountSeed.SortCode,
                AccountNumber = accountSeed.AccountNumber
            });

            var seedVaultDataQuery = @"INSERT INTO Vault (Id, AccountId, CurrentBalance, Currency) VALUES (@Id, @AccountId, @Amount, @Currency);";
            _context.Connection.Execute(seedVaultDataQuery, new
            {
                vaultSeed.Id,
                vaultSeed.AccountId,
                vaultSeed.CurrentBalance.Amount,
                vaultSeed.CurrentBalance.Currency
            });

            var seedCustomerAccountDataQuery = @"INSERT INTO CustomerAccount (Id, CustomerId, AccountId) VALUES (@Id, @CustomerId, @AccountId);";
            _context.Connection.Execute(seedCustomerAccountDataQuery, new
            {
                customerAccountSeed.Id,
                customerAccountSeed.CustomerId,
                customerAccountSeed.AccountId
            });

            return Task.CompletedTask;
        }

        [Fact]
        public async Task AddAsync_ShouldReturnTrue_WhenCustomerAccountIsAdded()
        {
            // Arrange
            var customerAccountRepository = new CustomerAccountRepository(_context);
            var customerAccount = new CustomerAccount(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            // Act
            var result = await customerAccountRepository.AddAsync(customerAccount);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenCustomerAccountIsDeleted()
        {
            // Arrange
            var customerAccountRepository = new CustomerAccountRepository(_context);
            var customerAccount = new CustomerAccount(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            await customerAccountRepository.AddAsync(customerAccount);

            // Act
            var result = await customerAccountRepository.DeleteAsync(customerAccount.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetByCustomerAccountIdAsync_ShouldReturnCustomerAccount()
        {
            // Arrange
            var customerAccountRepository = new CustomerAccountRepository(_context);
            var customerAccount = SqliteTestHelper.customerAccountSeed;

            // Act
            var result = await customerAccountRepository.GetByCustomerAccountIdAsync(customerAccount.Id);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByCustomerIdAsync_ShouldReturnCustomerAccounts()
        {
            // Arrange
            var customerAccountRepository = new CustomerAccountRepository(_context);
            var customerAccount = SqliteTestHelper.customerAccountSeed;
            var customerAccount1 = new CustomerAccount(Guid.NewGuid(), customerAccount.CustomerId, Guid.NewGuid());
            await customerAccountRepository.AddAsync(customerAccount1);

            // Act
            var result = await customerAccountRepository.GetByCustomerIdAsync(customerAccount.CustomerId);

            // Assert
            Assert.NotNull(result);
            result.Count().Should().Be(2);  
        }

        [Fact]
        public async Task GetByAccountIdAsync_ShouldReturnCustomerAccount()
        {
            // Arrange
            var customerAccountRepository = new CustomerAccountRepository(_context);
            var customerAccount = SqliteTestHelper.customerAccountSeed;

            // Act
            var result = await customerAccountRepository.GetByAccountIdAsync(customerAccount.AccountId);

            //Assert
            result.Should().NotBeNull();
        }
    }
}
