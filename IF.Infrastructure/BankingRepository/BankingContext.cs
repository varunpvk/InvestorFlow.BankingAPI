using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace IF.Infrastructure.BankingRepository
{
    public class BankingContext : IDisposable
    {
        private readonly IDbConnection _connection;
        private IDbTransaction _transaction;

        public BankingContext(IConfiguration configuration)
        {
            _connection = new SqliteConnection(configuration.GetConnectionString("DefaultConnection"));
            _connection.Open();
        }

        public IDbConnection Connection => _connection;
        public IDbTransaction Transaction => _transaction;

        public void BeginTransaction()
        {
            if(_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            _transaction = _connection.BeginTransaction();
        }   

        public void Commit()
        {
            _transaction?.Commit();
        }

        public void Rollback()
        {
            _transaction?.Rollback();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }
}
