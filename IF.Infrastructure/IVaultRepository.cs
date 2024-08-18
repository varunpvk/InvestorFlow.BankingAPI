using IF.Domain.DTOs;
using IF.Domain.Entities;

namespace IF.Infrastructure
{
    public interface IVaultRepository
    {
        Task<VaultDTO> GetAsync(Guid id);
        Task<VaultDTO> GetByAccountIdAsync(Guid accountId);
        Task<bool> AddAsync(Vault vault);
        Task<bool> UpdateAsync(Vault vault);
        Task<bool> DeleteAsync(Guid id);
    }
}
