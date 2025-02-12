using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBankAPI.Infrastructure.Persistence;
using ProjectBankAPI.Domain.Models;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectBankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly BankingDbContext _context;

        public ClientsController(BankingDbContext context)
        {
            _context = context;
        }

        // GET: api/Clients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            return await _context.Clients
                .Include(c => c.BankAccounts) // Incluir cuentas bancarias
                .ToListAsync();
        }

        // GET: api/Clients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _context.Clients
                .Include(c => c.BankAccounts) // Incluir cuentas bancarias
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
            {
                return NotFound(new { message = "Cliente no encontrado." });
            }

            return client;
        }

        // PUT: api/Clients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(int id, Client client)
        {
            Log.Information("Solicitud PUT recibida para actualizar cliente con ID {ClientId}", id);

            if (id != client.Id)
            {
                Log.Warning("El ID del cliente no coincide. ID en URL: {ClientId}, ID en cuerpo: {BodyId}", id, client.Id);
                return BadRequest(new { message = "El ID del cliente no coincide." });
            }

            // Verificar si el email ya está registrado por otro cliente
            var existingClient = await _context.Clients.FirstOrDefaultAsync(c => c.Email == client.Email && c.Id != id);
            if (existingClient != null)
            {
                Log.Warning("Intento de actualizar cliente con un email ya registrado: {Email}", client.Email);
                return BadRequest(new { message = "El correo electrónico ya está en uso." });
            }

            _context.Entry(client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                Log.Information("Cliente con ID {ClientId} actualizado correctamente", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
                {
                    Log.Error("Intento de actualizar un cliente no encontrado con ID {ClientId}", id);
                    return NotFound(new { message = "Cliente no encontrado." });
                }
                else
                {
                    Log.Error("Error de concurrencia al actualizar el cliente con ID {ClientId}", id);
                    throw;
                }
            }

            Log.Information("Respuesta enviada: NoContent para el cliente con ID {ClientId}", id);
            return BadRequest(new { message = "Información actualizada" });
            //return NoContent();
        }


        // POST: api/Clients
        [HttpPost]
        public async Task<ActionResult<Client>> PostClient(Client client)
        {
            // Verificar si el correo ya existe
            var existingClient = await _context.Clients.FirstOrDefaultAsync(c => c.Email == client.Email);
            if (existingClient != null)
            {
                return BadRequest(new { message = "El correo electrónico ya está registrado." });
            }

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
        }

        // DELETE: api/Clients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound(new { message = "Cliente no encontrado." });
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return BadRequest(new { message = "Eliminación exitosa." });
            //return NoContent();
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}
