using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBankAPI.Data;
using ProjectBankAPI.Models;
using Serilog;
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
            Log.Information("Solicitud GET: Listado de transacciones");

            var transactions = await _context.Transactions
                .Include(t => t.BankAccount)
                .ToListAsync();

            Log.Information("Se encontraron {Count} transacciones", transactions.Count);
            return transactions;
        }

        // GET: api/Transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            Log.Information("Solicitud GET: Obtener transacción con ID {TransactionId}", id);

            var transaction = await _context.Transactions
                .Include(t => t.BankAccount)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
            {
                Log.Warning("Transacción con ID {TransactionId} no encontrada", id);
                return NotFound(new { message = "Transacción no encontrada." });
            }

            Log.Information("Transacción con ID {TransactionId} encontrada", id);
            return transaction;
        }

        // POST: api/Transactions
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)
        {
            Log.Information("Solicitud POST: Creación de transacción {@Transaction}", transaction);

            using var transactionScope = await _context.Database.BeginTransactionAsync();
            try
            {
                var account = await _context.BankAccounts.FindAsync(transaction.AccountId);
                if (account == null)
                {
                    Log.Warning("Error: La cuenta origen {AccountId} no existe", transaction.AccountId);
                    return BadRequest(new { message = "La cuenta origen no existe." });
                }

                if (!Enum.IsDefined(typeof(TransactionType), transaction.Type))
                {
                    Log.Warning("Error: Tipo de transacción inválido ({TransactionType})", transaction.Type);
                    return BadRequest(new { message = "El tipo de transacción es inválido." });
                }

                if (transaction.Type == TransactionType.Withdrawal)
                {
                    if (account.Balance < transaction.Amount)
                    {
                        Log.Warning("Error: Saldo insuficiente en cuenta {AccountId} para retiro de {Amount}", transaction.AccountId, transaction.Amount);
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
                        Log.Warning("Error: No se especificó una cuenta destino para la transferencia desde la cuenta {AccountId}", transaction.AccountId);
                        return BadRequest(new { message = "Debe especificar una cuenta destino para la transferencia." });
                    }

                    var destinationAccount = await _context.BankAccounts.FindAsync(transaction.DestinationAccountId);
                    if (destinationAccount == null)
                    {
                        Log.Warning("Error: Cuenta destino {DestinationAccountId} no encontrada", transaction.DestinationAccountId);
                        return BadRequest(new { message = "Cuenta destino no encontrada." });
                    }

                    if (account.Balance < transaction.Amount)
                    {
                        Log.Warning("Error: Saldo insuficiente en cuenta {AccountId} para transferir {Amount} a la cuenta {DestinationAccountId}", transaction.AccountId, transaction.Amount, transaction.DestinationAccountId);
                        return BadRequest(new { message = "Saldo insuficiente para la transferencia." });
                    }

                    if (transaction.AccountId == transaction.DestinationAccountId)
                    {
                        Log.Warning("Error: Transferencia entre la misma cuenta {AccountId} no permitida", transaction.AccountId);
                        return BadRequest(new { message = "No puedes transferir a la misma cuenta." });
                    }

                    account.Balance -= transaction.Amount;
                    destinationAccount.Balance += transaction.Amount;
                }

                transaction.BankAccount = account;
                transaction.Timestamp = DateTime.UtcNow;

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
                await transactionScope.CommitAsync();

                Log.Information("Transacción creada exitosamente con ID {TransactionId}", transaction.Id);
                return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
            }
            catch (Exception ex)
            {
                await transactionScope.RollbackAsync();
                Log.Error(ex, "Error al procesar la transacción {@Transaction}", transaction);
                return StatusCode(500, new { message = "Error al procesar la transacción", error = ex.Message });
            }
        }

        // DELETE: api/Transactions/5 (Opcional)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            Log.Information("Solicitud DELETE: Intento de eliminar transacción con ID {TransactionId}", id);

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                Log.Warning("Error: No se encontró la transacción con ID {TransactionId} para eliminar", id);
                return NotFound(new { message = "Transacción no encontrada." });
            }

            Log.Warning("Intento de eliminación de transacción no permitido: ID {TransactionId}", id);
            return BadRequest(new { message = "No se pueden eliminar transacciones registradas." });
        }
    }
}
