using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBankAPI.Infrastructure.Persistence;
using ProjectBankAPI.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectBankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankAccountsController : ControllerBase
    {
        private readonly BankingDbContext _context;

        public BankAccountsController(BankingDbContext context)
        {
            _context = context;
        }

        // GET: api/BankAccounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BankAccount>>> GetBankAccounts()
        {
            return await _context.BankAccounts
                .Include(b => b.Client) // Incluir cliente asociado
                .ToListAsync();
        }

        // GET: api/BankAccounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BankAccount>> GetBankAccount(int id)
        {
            var bankAccount = await _context.BankAccounts
                .Include(b => b.Client) // Incluir cliente asociado
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bankAccount == null)
            {
                return NotFound(new { message = "Cuenta bancaria no encontrada." });
            }

            return bankAccount;
        }

        // PUT: api/BankAccounts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBankAccount(int id, BankAccount bankAccount)
        {
            if (id != bankAccount.Id)
            {
                return BadRequest(new { message = "El ID de la cuenta bancaria no coincide." });
            }

            // Obtener la cuenta bancaria original
            var existingAccount = await _context.BankAccounts.FindAsync(id);
            if (existingAccount == null)
            {
                return NotFound(new { message = "Cuenta bancaria no encontrada." });
            }

            // No permitir cambiar el propietario de la cuenta
            if (existingAccount.ClientId != bankAccount.ClientId)
            {
                return BadRequest(new { message = "No se puede cambiar el propietario de la cuenta." });
            }

            _context.Entry(existingAccount).CurrentValues.SetValues(bankAccount);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "Error de concurrencia. Intente de nuevo." });
            }

            return NoContent();
        }

        // POST: api/BankAccounts
        [HttpPost]
        public async Task<ActionResult<BankAccount>> PostBankAccount(BankAccount bankAccount)
        {
            // Validar si el cliente existe
            var clientExists = await _context.Clients.AnyAsync(c => c.Id == bankAccount.ClientId);
            if (!clientExists)
            {
                return BadRequest(new { message = "El cliente especificado no existe." });
            }

            // Verificar si el número de cuenta ya está en uso
            var accountExists = await _context.BankAccounts.AnyAsync(b => b.AccountNumber == bankAccount.AccountNumber);
            if (accountExists)
            {
                return BadRequest(new { message = "El número de cuenta ya está en uso." });
            }

            // **Forzar balance inicial a 0**
            bankAccount.Balance = 0m;

            _context.BankAccounts.Add(bankAccount);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBankAccount), new { id = bankAccount.Id }, bankAccount);
        }

        // DELETE: api/BankAccounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBankAccount(int id)
        {
            var bankAccount = await _context.BankAccounts
                .Include(b => b.Transactions) // Incluir transacciones para validación
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bankAccount == null)
            {
                return NotFound(new { message = "Cuenta bancaria no encontrada." });
            }

            // No permitir eliminar cuentas con transacciones registradas
            if (bankAccount.Transactions.Any())
            {
                return BadRequest(new { message = "No se puede eliminar una cuenta con transacciones registradas." });
            }

            _context.BankAccounts.Remove(bankAccount);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BankAccountExists(int id)
        {
            return _context.BankAccounts.Any(e => e.Id == id);
        }
    }
}
