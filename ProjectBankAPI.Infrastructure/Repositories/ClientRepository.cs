using Microsoft.EntityFrameworkCore;
using ProjectBankAPI.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBankAPI.Infrastructure.Persistence.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly BankingDbContext _context;

        public ClientRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            return await _context.Clients.FindAsync(id);
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            return await _context.Clients.ToListAsync();
        }

        public async Task AddAsync(Client client)
        {
            await _context.Clients.AddAsync(client);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
