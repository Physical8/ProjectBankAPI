using MediatR;
using ProjectBankAPI.Infrastructure.Persistence.Repositories;
using ProjectBankAPI.Domain.Models;
using Serilog;

namespace ProjectBankAPI.Application.Commands.Transactions
{
    public class CreateTransactionHandler : IRequestHandler<CreateTransactionCommand, Transaction>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IBankAccountRepository _bankAccountRepository;

        public CreateTransactionHandler(ITransactionRepository transactionRepository, IBankAccountRepository bankAccountRepository)
        {
            _transactionRepository = transactionRepository;
            _bankAccountRepository = bankAccountRepository;
        }

        public async Task<Transaction> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            Log.Information("Solicitud de transacción recibida: {@Request}", request);

            if (request.Amount <= 0)
            {
                throw new Exception("El monto de la transacción debe ser mayor a cero.");
            }

            //  Validar que la cuenta origen exista
            var account = await _bankAccountRepository.GetByIdAsync(request.AccountId);
            if (account == null)
            {
                Log.Warning("Error: La cuenta origen con ID {AccountId} no existe", request.AccountId);
                throw new Exception($"La cuenta bancaria con ID {request.AccountId} no existe.");
            }

            //  Validar que el tipo de transacción sea válido
            if (!Enum.IsDefined(typeof(TransactionType), request.Type))
            {
                Log.Warning("Error: Tipo de transacción inválido ({TransactionType})", request.Type);
                throw new Exception("El tipo de transacción es inválido.");
            }

            //  Aplicar lógica de negocio según el tipo de transacción
            if (request.Type == TransactionType.Withdrawal) //  Retiro
            {
                if (account.Balance < request.Amount)
                {
                    Log.Warning("Error: Saldo insuficiente en cuenta {AccountId} para retiro de {Amount}", request.AccountId, request.Amount);
                    throw new Exception("Saldo insuficiente para retiro.");
                }
                account.Balance -= request.Amount;
                Log.Information("Retiro exitoso: {Amount} descontado de cuenta {AccountId}", request.Amount, request.AccountId);
            }
            else if (request.Type == TransactionType.Deposit) //  Consignación
            {
                account.Balance += request.Amount;
                Log.Information("Consignación exitosa: {Amount} agregado a cuenta {AccountId}", request.Amount, request.AccountId);
            }
            else if (request.Type == TransactionType.Transfer) //  Transferencia
            {
                if (request.DestinationAccountId == null)
                {
                    Log.Warning("Error: No se especificó una cuenta destino para la transferencia desde la cuenta {AccountId}", request.AccountId);
                    throw new Exception("Debe especificar una cuenta destino para la transferencia.");
                }

                var destinationAccount = await _bankAccountRepository.GetByIdAsync(request.DestinationAccountId.Value);
                if (destinationAccount == null)
                {
                    Log.Warning("Error: Cuenta destino con ID {DestinationAccountId} no encontrada", request.DestinationAccountId);
                    throw new Exception("Cuenta destino no encontrada.");
                }

                if (account.Balance < request.Amount)
                {
                    Log.Warning("Error: Saldo insuficiente en cuenta {AccountId} para transferir {Amount} a cuenta {DestinationAccountId}", request.AccountId, request.Amount, request.DestinationAccountId);
                    throw new Exception("Saldo insuficiente para la transferencia.");
                }

                if (request.AccountId == request.DestinationAccountId)
                {
                    Log.Warning("Error: No se puede transferir entre la misma cuenta {AccountId}", request.AccountId);
                    throw new Exception("No puedes transferir a la misma cuenta.");
                }

                account.Balance -= request.Amount;
                destinationAccount.Balance += request.Amount;
                Log.Information("Transferencia exitosa: {Amount} transferido de cuenta {AccountId} a cuenta {DestinationAccountId}", request.Amount, request.AccountId, request.DestinationAccountId);
            }

            //  Guardar cambios en cuentas y transacción
            var transaction = new Transaction
            {
                AccountId = request.AccountId,
                BankAccount = account,
                Amount = request.Amount,
                Type = request.Type,
                DestinationAccountId = request.DestinationAccountId,
                Timestamp = DateTime.UtcNow
            };

            await _transactionRepository.AddAsync(transaction);
            await _bankAccountRepository.SaveChangesAsync();
            await _transactionRepository.SaveChangesAsync();

            Log.Information("Transacción registrada exitosamente con ID {TransactionId}", transaction.Id);
            return transaction;
        }
    }
}
