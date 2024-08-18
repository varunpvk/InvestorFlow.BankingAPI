using IF.Domain.Abstractions;
using IF.Domain.ValueObjects;

namespace IF.Domain.Entities
{
    public sealed class Customer : Entity
    {
        public Customer(
            Guid id,
            string name,
            string emailId,
            string phoneNo,
            Account account,
            Address address)
            : base(id)
        {
            Name = name;
            EmailId = emailId;
            PhoneNo = phoneNo;
            Account = account;
            Address = address;
        }

        public Account Account { get; private set; }
        public string Name { get; private set; }
        public string EmailId { get; private set; }
        public Address Address { get; private set; }
        public string PhoneNo { get; private set; }
    }
}
