using Microsoft.EntityFrameworkCore;
using ProjectBankAPI.Domain.Models;

namespace ProjectBankAPI.Infrastructure.Persistence.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly BankingDbContext _context;

        public TransactionRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return await _context.Transactions.Include(t => t.BankAccount).ToListAsync();
        }

        public async Task<Transaction?> GetByIdAsync(int id)
        {
            return await _context.Transactions.Include(t => t.BankAccount)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task AddAsync(Transaction transaction)
        {
            // ✅ Asegurar que el objeto BankAccount no esté null antes de agregar la transacción
            if (transaction.BankAccount == null)
            {
                transaction.BankAccount = await _context.BankAccounts.FindAsync(transaction.AccountId);
            }

            await _context.Transactions.AddAsync(transaction);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
