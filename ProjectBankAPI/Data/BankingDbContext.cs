using Microsoft.EntityFrameworkCore;
using ProjectBankAPI.Models;

namespace ProjectBankAPI.Data
{
    public class BankingDbContext: DbContext
    {
        public BankingDbContext(DbContextOptions<BankingDbContext> options) : base(options) { }
        public DbSet<Client> Clients { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            // Configura la precisión de las propiedades decimal
            modelBuilder.Entity<BankAccount>()
                .Property(b => b.Balance)
                .HasPrecision(18, 2); // 18 dígitos en total, 2 decimales

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2); // 18 dígitos en total, 2 decimales

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Timestamp)
                .HasDefaultValueSql("GETUTCDATE()"); // Asegura que la base de datos use la fecha UTC actual

            modelBuilder.Entity<Transaction>()
                .Property(t => t.DestinationAccountId)
                .IsRequired(false); // Permitir valores nulos en DestinationAccountId
        }
    }
}
