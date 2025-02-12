using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBankAPI.Domain.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }  // Cuenta origen
        public decimal Amount { get; set; } // Monto de la transacción
        public TransactionType Type { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] // No aparece si es null
        public int? DestinationAccountId { get; set; } // Solo para transferencias

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // Fecha de la transacción

        [JsonIgnore] // Evita referencia circular en Swagger
        public BankAccount? BankAccount { get; set; }
    }

    public enum TransactionType
    {
        Deposit,    // Consignación
        Withdrawal, // Retiro
        Transfer    // Transferencia
    }
}
