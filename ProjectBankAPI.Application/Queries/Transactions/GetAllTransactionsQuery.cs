using MediatR;
using ProjectBankAPI.Domain.Models;

namespace ProjectBankAPI.Queries.Transactions
{
    public class GetAllTransactionsQuery : IRequest<IEnumerable<Transaction>> { }
}