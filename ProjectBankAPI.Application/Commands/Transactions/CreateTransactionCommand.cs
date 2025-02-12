using MediatR;
using ProjectBankAPI.Domain.Models;

namespace ProjectBankAPI.Application.Commands.Transactions
{
    public class CreateTransactionCommand : IRequest<Transaction>
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public int? DestinationAccountId { get; set; }
    }
}
