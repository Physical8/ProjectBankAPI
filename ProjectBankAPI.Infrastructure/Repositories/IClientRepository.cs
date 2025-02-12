using ProjectBankAPI.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
