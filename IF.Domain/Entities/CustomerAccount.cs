using IF.Domain.Abstractions;

namespace IF.Domain.Entities
{
    public class CustomerAccount : Entity
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public Guid AccountId { get; private set; }
        public IList<Guid> Transactions { get; private set; }

        public CustomerAccount(
            Guid id, 
            Guid customerId,
            Guid accountId)
            : base(id)
        {
            Id = id;
            CustomerId = customerId;
            AccountId = accountId;
            Transactions = CreateEmptyTransactionList();
        }

        private IList<Guid> CreateEmptyTransactionList() => new List<Guid>();   
    }
}
