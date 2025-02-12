using ProjectBankAPI.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
