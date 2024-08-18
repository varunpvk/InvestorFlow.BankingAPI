using Dapper;
using IF.Domain.Entities;
using IF.Domain.Enums;
using IF.Domain.ValueObjects;

namespace IF.Infrastructure.IntegrationTests.Helper
{
    public class SqliteTestHelper : IntegrationTestBase
    {
        public static readonly Account accountSeed = new Account(Guid.NewGuid(), AccountType.Savings);
        public static readonly Account accountSeed1 = new Account(Guid.NewGuid(), AccountType.Savings);
        public static readonly Balance balanceSeed = new Balance(1000, "GBP");
        public static readonly Vault vaultSeed = new Vault(Guid.NewGuid(), accountSeed.Id);
        public static readonly CustomerAccount customerAccountSeed = new CustomerAccount(Guid.NewGuid(), Guid.NewGuid(), accountSeed.Id);
        public static readonly TransactionDetails transactionDetailsSeed = new TransactionDetails(
            TransactionType.Deposit, 
            balanceSeed.Amount,
            balanceSeed.Currency);
        public static readonly Transaction transactionSeed = new Transaction(
            Guid.NewGuid(),
            customerAccountSeed.Id,
            transactionDetailsSeed,
            DateTime.UtcNow
        );
        
        public SqliteTestHelper()
        {
            CreateTables();
        }

        private void CreateTables()
        {
            var createTableCommand = @"
                CREATE TABLE IF NOT EXISTS Account (
                    Id TEXT PRIMARY KEY,
                    Name TEXT,
                    SortCode TEXT,
                    AccountNumber TEXT
                );
                CREATE TABLE IF NOT EXISTS Vault (
                    Id TEXT PRIMARY KEY,
                    AccountId TEXT,
                    CurrentBalance REAL,
                    Currency TEXT,
                    FOREIGN KEY (AccountId) REFERENCES Account(Id)
                );
                CREATE TABLE IF NOT EXISTS TransactionHistory (
                    Id TEXT PRIMARY KEY,
                    CustomerAccountId TEXT,
                    Type TEXT,
                    Amount Real,
                    Currency TEXT,
                    Description TEXT,
                    TransactionDate TEXT
                );
                CREATE TABLE IF NOT EXISTS CustomerAccount (
                    Id TEXT PRIMARY KEY,
                    CustomerId TEXT,
                    AccountId TEXT
                );";
            Context.Connection.Execute(createTableCommand);
        }

        private void DropTables()
        {
            var dropTablesQuery = @"DROP TABLE IF EXISTS TransactionHistory;
                                    DROP TABLE IF EXISTS Vault;
                                    DROP TABLE IF EXISTS Account;
                                    DROP TABLE IF EXISTS CustomerAccount;";
            Context.Connection.Execute(dropTablesQuery);
        }

        public void Dispose()
        {
            DropTables();
        }
    }
}
