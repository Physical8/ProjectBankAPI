﻿using System.Text.Json.Serialization;

namespace ProjectBankAPI.Domain.Models
{
    public class Client
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }

        // Relación con cuentas bancarias (Un cliente puede tener varias cuentas) test git

        [JsonIgnore] // Evita la serialización circular
        public List<BankAccount> BankAccounts { get; set; } = new();
    }
}
