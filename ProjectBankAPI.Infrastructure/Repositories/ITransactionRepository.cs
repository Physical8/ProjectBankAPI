using ProjectBankAPI.Domain.Models;

namespace ProjectBankAPI.Infrastructure.Persistence.Repositories
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetAllAsync();
        Task<Transaction?> GetByIdAsync(int id);
        Task AddAsync(Transaction transaction);
        Task SaveChangesAsync();
    }
}
