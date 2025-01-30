using ProjectBankAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBankAPI.Repositories
{
    public interface IBankAccountRepository
    {
        Task<BankAccount?> GetByIdAsync(int id);
        Task<IEnumerable<BankAccount>> GetAllAsync();
        Task AddAsync(BankAccount account);
        Task SaveChangesAsync();
    }
}
