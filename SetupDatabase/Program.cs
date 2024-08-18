// See https://aka.ms/new-console-template for more information
using Microsoft.Data.Sqlite;

string connectionString = "DataSource=Banking.db";
using(var connection = new SqliteConnection(connectionString))
{
    connection.Open();
    InitializeDatabase(connection);
}

void InitializeDatabase(SqliteConnection sqliteConnection)
{
    var command = sqliteConnection.CreateCommand();
    command.CommandText = @"CREATE TABLE IF NOT EXISTS Account (
                                Id TEXT PRIMARY KEY, 
                                Name TEXT, 
                                SortCode TEXT, 
                                AccountNumber TEXT
                            );";
    command.ExecuteNonQuery();
    command.CommandText = @"CREATE TABLE IF NOT EXISTS Vault (
                                Id TEXT PRIMARY KEY, 
                                AccountId TEXT,
                                CurrentBalance REAL,            
                                Currency TEXT,  
                                FOREIGN KEY (AccountId) REFERENCES Account(Id)
                            );";
    command.ExecuteNonQuery();
    command.CommandText = @"CREATE TABLE IF NOT EXISTS TransactionHistory (
                                Id TEXT PRIMARY KEY, 
                                CustomerAccountId TEXT,
                                Type TEXT,
                                Amount Real,
                                Currency TEXT,
                                Description TEXT,
                                TransactionDate TEXT
                            );";
    command.ExecuteNonQuery();
    command.CommandText = @"CREATE TABLE IF NOT EXISTS CustomerAccount (
                                Id TEXT PRIMARY KEY, 
                                CustomerId TEXT,
                                AccountId TEXT
                            );";
    command.ExecuteNonQuery();
}
