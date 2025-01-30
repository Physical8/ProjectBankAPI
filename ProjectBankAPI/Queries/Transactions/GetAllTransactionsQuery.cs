using MediatR;
using ProjectBankAPI.Models;
using System.Collections.Generic;

namespace ProjectBankAPI.Queries.Transactions
{
    public class GetAllTransactionsQuery : IRequest<IEnumerable<Transaction>> { }
}