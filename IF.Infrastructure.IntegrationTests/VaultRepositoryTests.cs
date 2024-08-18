using Dapper;
using IF.Domain.DTOs;
using IF.Domain.Entities;
using IF.Domain.ValueObjects;
using IF.Infrastructure.BankingRepository;
using IF.Infrastructure.IntegrationTests.Fixtures;
using IF.Infrastructure.IntegrationTests.Helper;

namespace IF.Infrastructure.IntegrationTests
{
    [Collection("SequentialTests")]
    public class VaultRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly BankingContext _context;

        public VaultRepositoryTests(DatabaseFixture fixture)
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
            CleanDatabase();
        }

        private void CleanDatabase()
        {
            _context.Connection.Execute("DELETE FROM Vault;");
            _context.Connection.Execute("DELETE FROM Account;");
        }

        private void SeedData()
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
        }

        [Fact]
        public async Task AddVaultAsync_ShouldReturnTrue_WhenVaultIsAdded()
        {
            // Arrange
            var vaultRepository = new VaultRepository(_context);
            var account = SqliteTestHelper.accountSeed;
            var vault = new Vault(Guid.NewGuid(), account.Id);

            // Act
            var result = await vaultRepository.AddAsync(vault);

            // Assert
            Assert.True(result);
            var addedVault = await _context.Connection.QueryAsync<VaultDTO>(
                "SELECT * FROM Vault WHERE Id = @Id",
                new { vault.Id });
            Assert.NotNull(addedVault);
        }

        [Fact]
        public async Task GetVaultByIdAsync_ShouldReturnVaultDetails()
        {
            // Arrange
            var vault = SqliteTestHelper.vaultSeed;
            var vaultRepository = new VaultRepository(_context);

            // Act
            var result = await vaultRepository.GetAsync(vault.Id);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateVaultAsync_ShouldUpdateVault()
        {
            // Arrange
            var vault = SqliteTestHelper.vaultSeed;
            vault.UpdateVault(2000, "GBP");
            var vaultRepository = new VaultRepository(_context);

            // Act
            var result = await vaultRepository.UpdateAsync(vault);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteVaultAsync_ShouldDeleteVault()
        {
            // Arrange
            var vault = new Vault(Guid.NewGuid(), SqliteTestHelper.accountSeed.Id);
            var vaultRepository = new VaultRepository(_context);
            await vaultRepository.AddAsync(vault);

            // Act
            var result = await vaultRepository.DeleteAsync(vault.Id);

            // Assert
            Assert.True(result);
        }

        
    }
}
