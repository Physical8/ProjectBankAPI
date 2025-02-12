using MediatR;
using ProjectBankAPI.Domain.Models;

namespace ProjectBankAPI.Queries.Transactions
{
    public class GetTransactionByIdQuery : IRequest<Transaction?>
    {
        public int Id { get; }

        public GetTransactionByIdQuery(int id)
        {
            Id = id;
        }
    }
}
