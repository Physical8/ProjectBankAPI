using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBankAPI.Data;
using ProjectBankAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectBankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly BankingDbContext _context;

        public TransactionsController(BankingDbContext context)
        {
            _context = context;
        }

        // GET: api/Transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            return await _context.Transactions
                .Include(t => t.BankAccount) // Incluir cuenta origen
                .ToListAsync();
        }

        // GET: api/Transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.BankAccount) // Incluir cuenta origen
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
            {
                return NotFound(new { message = "Transacción no encontrada." });
            }

            return transaction;
        }

        // POST: api/Transactions
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)
        {
            using var transactionScope = await _context.Database.BeginTransactionAsync();

            try
            {
                var account = await _context.BankAccounts.FindAsync(transaction.AccountId);
                if (account == null)
                {
                    return BadRequest(new { message = "La cuenta origen no existe." });
                }

                if (!Enum.IsDefined(typeof(TransactionType), transaction.Type))
                {
                    return BadRequest(new { message = "El tipo de transacción es inválido." });
                }

                if (transaction.Type == TransactionType.Withdrawal)
                {
                    if (account.Balance < transaction.Amount)
                    {
                        return BadRequest(new { message = "Saldo insuficiente." });
                    }
                    account.Balance -= transaction.Amount;
                }
                else if (transaction.Type == TransactionType.Deposit)
                {
                    account.Balance += transaction.Amount;
                }
                else if (transaction.Type == TransactionType.Transfer)
                {
                    if (transaction.DestinationAccountId == null)
                    {
                        return BadRequest(new { message = "Debe especificar una cuenta destino para la transferencia." });
                    }

                    var destinationAccount = await _context.BankAccounts.FindAsync(transaction.DestinationAccountId);
                    if (destinationAccount == null)
                    {
                        return BadRequest(new { message = "Cuenta destino no encontrada." });
                    }

                    if (account.Balance < transaction.Amount)
                    {
                        return BadRequest(new { message = "Saldo insuficiente para la transferencia." });
                    }

                    if (transaction.AccountId == transaction.DestinationAccountId)
                    {
                        return BadRequest(new { message = "No puedes transferir a la misma cuenta." });
                    }

                    account.Balance -= transaction.Amount;
                    destinationAccount.Balance += transaction.Amount;
                }

                // ✅ Asignar explícitamente la cuenta antes de guardar la transacción
                transaction.BankAccount = account;
                transaction.Timestamp = DateTime.UtcNow;

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
                await transactionScope.CommitAsync();

                return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
            }
            catch (Exception ex)
            {
                await transactionScope.RollbackAsync();
                return StatusCode(500, new { message = "Error al procesar la transacción", error = ex.Message });
            }
        }



        // DELETE: api/Transactions/5 (Opcional)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound(new { message = "Transacción no encontrada." });
            }

            // No permitir eliminar transacciones que afectan los saldos
            return BadRequest(new { message = "No se pueden eliminar transacciones registradas." });
        }
    }
}
