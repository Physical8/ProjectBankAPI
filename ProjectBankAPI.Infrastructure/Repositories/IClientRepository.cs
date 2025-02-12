using ProjectBankAPI.Domain.Models;

namespace ProjectBankAPI.Infrastructure.Persistence.Repositories
{
    public interface IClientRepository
    {
        Task<Client?> GetByIdAsync(int id);
        Task<IEnumerable<Client>> GetAllAsync();
        Task AddAsync(Client client);
        Task SaveChangesAsync();
    }
}
