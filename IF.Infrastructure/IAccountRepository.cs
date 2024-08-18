using IF.Domain.DTOs;
using IF.Domain.Entities;

namespace IF.Infrastructure
{
    public interface IAccountRepository
    {
        Task<AccountDTO> GetAsync(Guid id);
        Task<bool> AddAsync(Account account);
        Task<bool> UpdateAsync(Account account);
        Task<bool> DeleteAsync(Guid id);
    }
}
