using MediatR;
using ProjectBankAPI.Models;
using ProjectBankAPI.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectBankAPI.Queries.Transactions
{
    public class GetTransactionByIdHandler : IRequestHandler<GetTransactionByIdQuery, Transaction?>
    {
        private readonly ITransactionRepository _repository;

        public GetTransactionByIdHandler(ITransactionRepository repository)
        {
            _repository = repository;
        }

        public async Task<Transaction?> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetByIdAsync(request.Id);
        }
    }
}
