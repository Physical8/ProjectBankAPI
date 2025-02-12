using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ProjectBankAPI.Infrastructure.Persistence;
using ProjectBankAPI.Infrastructure.Persistence.Repositories;

namespace ProjectBankAPI.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BankingDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Connection")));

            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IBankAccountRepository, BankAccountRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();

            return services;
        }
    }
}
