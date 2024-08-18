namespace IF.Infrastructure.BankingRepository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BankingContext _context;

        public ICustomerAccountRepository CustomerAccounts { get; set; }
        public IAccountRepository Accounts { get; private set; }
        public IVaultRepository Vaults { get; private set; }
        public ITransactionRepository Transactions { get; private set; }


        public UnitOfWork(BankingContext context)
        {
            _context = context;
            CustomerAccounts = new CustomerAccountRepository(_context);
            Accounts = new AccountRepository(_context);
            Vaults = new VaultRepository(_context);
            Transactions = new TransactionRepository(_context);
        }

        public void BeginTransaction()
        {
            _context.BeginTransaction();
        }

        public void Commit()
        {
            _context.Commit();
        }

        public void Rollback()
        {
            _context.Rollback();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
