using Dapper;
using IF.Domain.DTOs;
using IF.Domain.Entities;
using IF.Domain.Enums;
using IF.Domain.ValueObjects;
using IF.Infrastructure.BankingRepository;
using IF.Infrastructure.IntegrationTests.Fixtures;
using IF.Infrastructure.IntegrationTests.Helper;

namespace IF.Infrastructure.IntegrationTests
{
    public class TransactionRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly BankingContext _context;

        public TransactionRepositoryTests(DatabaseFixture fixture)
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
            _context.Connection.Execute("DELETE FROM TransactionHistory;");
            _context.Connection.Execute("DELETE FROM Vault;");
            _context.Connection.Execute("DELETE FROM Account;");
            return Task.CompletedTask;
        }

        private Task SeedData()
        {
            var accountSeed = SqliteTestHelper.accountSeed;
            
            
            var seedAccountDataQuery = @"INSERT INTO Account (Id, Name, SortCode, AccountNumber) VALUES (@Id, @Name, @SortCode, @AccountNumber);";
            _context.Connection.Execute(seedAccountDataQuery, new
            {
                Id = accountSeed.Id,
                Name = accountSeed.Type,
                SortCode = accountSeed.SortCode,
                AccountNumber = accountSeed.AccountNumber
            });

            var vaultSeed = SqliteTestHelper.vaultSeed;
            var seedVaultDataQuery = @"INSERT INTO Vault (Id, AccountId, CurrentBalance, Currency) VALUES (@Id, @AccountId, @Amount, @Currency);";
            _context.Connection.Execute(seedVaultDataQuery, new
            {
                vaultSeed.Id,
                vaultSeed.AccountId,
                vaultSeed.CurrentBalance.Amount,
                vaultSeed.CurrentBalance.Currency
            });

            var transactionSeed = SqliteTestHelper.transactionSeed;
            var seedTransactionDataQuery = @"INSERT INTO TransactionHistory (Id, CustomerAccountId, Type, Amount, Currency, Description, TransactionDate) VALUES (@Id, @CustomerAccountId, @Type, @Amount, @Currency, @Description, @TransactionDate);";
            _context.Connection.Execute(seedTransactionDataQuery, new
            {
                Id = transactionSeed.Id,
                CustomerAccountId = transactionSeed.CustomerAccountId,
                Type = transactionSeed.Details.Type,
                Amount = transactionSeed.Details.Amount,
                Currency = transactionSeed.Details.Currency,
                Description = transactionSeed.Details.Description,
                TransactionDate = transactionSeed.TransactionDateTimeUtc
            });

            return Task.CompletedTask;
        }

        [Fact]
        public async Task AddTransactionAsync_ShouldReturnTrue_WhenTransactionIsAdded()
        {
            // Arrange
            var transactionRepository = new TransactionRepository(_context);
            var transactionDetails = new TransactionDetails(TransactionType.Deposit, 100, "GBP");
            var transaction = new Transaction(
                Guid.NewGuid(),
                Guid.NewGuid(),
                transactionDetails,
                DateTime.UtcNow);

            // Act
            var result = await transactionRepository.AddAsync(transaction);

            // Assert
            Assert.True(result);
            var addedTransaction = await _context.Connection.QueryFirstOrDefaultAsync<TransactionDTO>(
                               "SELECT * FROM TransactionHistory WHERE Id = @Id",
                                              new { transaction.Id });
            Assert.NotNull(addedTransaction);
        }

        [Fact]
        public async Task GetTransactionByIdAsync_ShouldReturnTransaction()
        {
            // Arrange
            var transaction = SqliteTestHelper.transactionSeed;
            var transactionRepository = new TransactionRepository(_context);

            // Act
            var result = await transactionRepository.GetByIdAsync(transaction.Id);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetTransactionByAccountIdAsync_ShouldReturnTransaction()
        {
            // Arrange
            var customerAccount = SqliteTestHelper.customerAccountSeed;
            var transactionRepository = new TransactionRepository(_context);

            // Act
            var result = await transactionRepository.GetByAccountIdAsync(customerAccount.Id);

            // Assert
            Assert.NotNull(result);
        }
    }
}
