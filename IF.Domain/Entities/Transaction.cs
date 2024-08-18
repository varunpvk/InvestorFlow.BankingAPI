using IF.Domain.Abstractions;
using IF.Domain.ValueObjects;

namespace IF.Domain.Entities
{
    public sealed class Transaction : Entity
    {
        public Transaction(
            Guid id,
            Guid customerAccountId,
            TransactionDetails details,
            DateTime transactionDateTimeUtc)
            : base(id)
        {
            CustomerAccountId = customerAccountId;    
            Details = details;
            TransactionDateTimeUtc = transactionDateTimeUtc;
        }
        public Guid CustomerAccountId { get; private set; }
        public TransactionDetails Details { get; private set; }
        public DateTime TransactionDateTimeUtc { get; private set; }
    }
}
