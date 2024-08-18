namespace IF.Infrastructure.BankingRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICustomerAccountRepository CustomerAccounts { get; }
        IAccountRepository Accounts { get; }
        IVaultRepository Vaults { get; }
        ITransactionRepository Transactions { get; }
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
