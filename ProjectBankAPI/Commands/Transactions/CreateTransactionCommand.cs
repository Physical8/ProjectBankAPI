using MediatR;
using ProjectBankAPI.Models;

namespace ProjectBankAPI.Commands.Transactions
{
    public class CreateTransactionCommand : IRequest<Transaction>
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public int? DestinationAccountId { get; set; }
    }
}
