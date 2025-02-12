using MediatR;
using ProjectBankAPI.Domain.Models;
using System.Collections.Generic;

namespace ProjectBankAPI.Queries.Transactions
{
    public class GetAllTransactionsQuery : IRequest<IEnumerable<Transaction>> { }
}