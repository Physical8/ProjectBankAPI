using ProjectBankAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBankAPI.Repositories
{
    public interface IClientRepository
    {
        Task<Client?> GetByIdAsync(int id);
        Task<IEnumerable<Client>> GetAllAsync();
        Task AddAsync(Client client);
        Task SaveChangesAsync();
    }
}
