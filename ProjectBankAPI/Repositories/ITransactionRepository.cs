using ProjectBankAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBankAPI.Repositories
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetAllAsync();
        Task<Transaction?> GetByIdAsync(int id);
        Task AddAsync(Transaction transaction);
        Task SaveChangesAsync();
    }
}
