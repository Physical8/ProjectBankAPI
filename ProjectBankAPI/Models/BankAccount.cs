using System.Text.Json.Serialization;

namespace ProjectBankAPI.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public int ClientId { get; set; }

        [JsonIgnore]
        public Client? Client { get; set; } // Permitir valores nulos
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public List<Transaction> Transactions { get; set; } = new();
    }
}
