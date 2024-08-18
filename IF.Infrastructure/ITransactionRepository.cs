using IF.Domain.DTOs;
using IF.Domain.Entities;

namespace IF.Infrastructure
{
    public interface ITransactionRepository
    {
        Task<bool> AddAsync(Transaction transaction);
        Task<TransactionDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<TransactionDTO>> GetByAccountIdAsync(Guid accountId);
        Task<bool> DeleteAsync(Guid Id);
    }
}
