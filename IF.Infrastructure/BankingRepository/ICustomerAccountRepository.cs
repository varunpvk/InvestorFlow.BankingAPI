using IF.Domain.DTOs;
using IF.Domain.Entities;

namespace IF.Infrastructure.BankingRepository
{
    public interface ICustomerAccountRepository
    {
        Task<bool> AddAsync(CustomerAccount customer);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<CustomerAccountDTO>> GetByCustomerIdAsync(Guid customerId);
        Task<CustomerAccountDTO> GetByCustomerAccountIdAsync(Guid customerAccountId);
        Task<CustomerAccountDTO> GetByAccountIdAsync(Guid accountId);
    }
}
