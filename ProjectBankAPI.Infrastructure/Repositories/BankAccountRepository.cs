using Microsoft.EntityFrameworkCore;
using ProjectBankAPI.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBankAPI.Infrastructure.Persistence.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly BankingDbContext _context;

        public BankAccountRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<BankAccount?> GetByIdAsync(int id)
        {
            return await _context.BankAccounts.FindAsync(id);
        }

        public async Task<IEnumerable<BankAccount>> GetAllAsync()
        {
            return await _context.BankAccounts.ToListAsync();
        }

        public async Task AddAsync(BankAccount account)
        {
            await _context.BankAccounts.AddAsync(account);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
