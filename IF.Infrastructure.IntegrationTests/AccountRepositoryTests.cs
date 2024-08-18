using Dapper;
using IF.Domain.DTOs;
using IF.Domain.Entities;
using IF.Domain.Enums;
using IF.Infrastructure.BankingRepository;
using IF.Infrastructure.IntegrationTests.Fixtures;
using IF.Infrastructure.IntegrationTests.Helper;

namespace IF.Infrastructure.IntegrationTests
{
    [Collection("SequentialTests")]
    public class AccountRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly BankingContext _context;

        public AccountRepositoryTests(DatabaseFixture fixture)
        {
            _context = fixture.TestHelper.Context;
        }

        public Task InitializeAsync()
        {
            SeedData();
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            CleanDatabase().Wait();
        }

        private Task CleanDatabase()
        {
            _context.Connection.Execute("DELETE FROM Vault;");
            _context.Connection.Execute("DELETE FROM Account;");
            return Task.CompletedTask;
        }

        private Task SeedData()
        {
            var accountSeed = SqliteTestHelper.accountSeed;
            var vaultSeed = SqliteTestHelper.vaultSeed;
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

            return Task.CompletedTask;
        }

        [Fact]
        public async Task AddAccountAsync_ShouldReturnTrue_WhenAccountIsAdded()
        {
            // Arrange
            var accountRepository = new AccountRepository(_context);
            var account = new Account(Guid.NewGuid(), AccountType.Savings);

            // Act
            var result = await accountRepository.AddAsync(account);

            // Assert
            Assert.True(result);
            var addedAccount = await _context.Connection.QueryAsync<AccountDTO>(
                "SELECT * FROM Account WHERE AccountNumber = @AccountNumber",
                new { account.AccountNumber });
            Assert.NotNull(addedAccount);
        }

        [Fact]
        public async Task GetAccountByIdAsync_ShouldReturnAccount()
        {
            // Arrange
            var account = SqliteTestHelper.accountSeed;
            var accountRepository = new AccountRepository(_context);

            // Act
            var result = await accountRepository.GetAsync(account.Id);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateAccountAsync_ShouldUpdateAccount()
        {
            // Arrange
            var account = SqliteTestHelper.accountSeed;
            var updateSortCode = new Account(account.Id, AccountType.Savings);
            var accountRepository = new AccountRepository(_context); 

            // Act
            var result = await accountRepository.UpdateAsync(updateSortCode);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAccountAsync_ShouldDeleteAccount()
        {
            // Arrange
            var account = new Account(Guid.NewGuid(), AccountType.Demat);
            var accountRepository = new AccountRepository(_context);
            await accountRepository.AddAsync(account);

            // Act
            var result = await accountRepository.DeleteAsync(account.Id);

            // Assert
            Assert.True(result);
        }
    }
}
