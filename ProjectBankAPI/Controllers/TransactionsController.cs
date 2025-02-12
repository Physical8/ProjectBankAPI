using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjectBankAPI.Application.Commands.Transactions;
using ProjectBankAPI.Domain.Models;
using ProjectBankAPI.Queries.Transactions;
using Serilog;

namespace ProjectBankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            Log.Information("Solicitud GET: Listado de transacciones");

            var transactions = await _mediator.Send(new GetAllTransactionsQuery());

            Log.Information("Se encontraron {Count} transacciones", transactions.Count());
            return Ok(transactions);
        }

        // GET: api/Transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            Log.Information("Solicitud GET: Obtener transacción con ID {TransactionId}", id);

            var transaction = await _mediator.Send(new GetTransactionByIdQuery(id));

            if (transaction == null)
            {
                Log.Warning("Transacción con ID {TransactionId} no encontrada", id);
                return NotFound(new { message = "Transacción no encontrada." });
            }

            Log.Information("Transacción con ID {TransactionId} encontrada", id);
            return Ok(transaction);
        }

        // POST: api/Transactions
        [HttpPost]
        public async Task<ActionResult<Transaction>> CreateTransaction(CreateTransactionCommand command)
        {
            Log.Information("Solicitud POST: Creación de transacción {@Transaction}", command);

            var transaction = await _mediator.Send(command);

            Log.Information("Transacción creada exitosamente con ID {TransactionId}", transaction.Id);
            //return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
            return Ok(new { message = "Transacción realizada con éxito." });
        }
    }
}
