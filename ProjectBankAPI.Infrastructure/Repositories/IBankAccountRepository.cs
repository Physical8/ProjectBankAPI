using ProjectBankAPI.Domain.Models;

namespace ProjectBankAPI.Infrastructure.Persistence.Repositories
{
    public interface IBankAccountRepository
    {
        Task<BankAccount?> GetByIdAsync(int id);
        Task<IEnumerable<BankAccount>> GetAllAsync();
        Task AddAsync(BankAccount account);
        Task SaveChangesAsync();
    }
}
