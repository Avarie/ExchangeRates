using ExchangeRates.Repository;
using ExchangeRates.Service;
using ExchangeRates.Sources;
using Microsoft.Extensions.DependencyInjection;

namespace ExchangeRates
{
    public static class StartupExtensions
    {
        public static void RegisterImplementations(this IServiceCollection services)
        {

            services.AddScoped<ICurrencyDataSource, ObmenkaKhSource>();
            services.AddScoped<ICurrencyDataSource, PrivatBankSource>();
            services.AddScoped<ICurrencyDataSource, NbuBankSource>();
            services.AddScoped<ICurrencyDataSource, InvestingComSource>();

            //services.AddScoped<ICurrencyDataSource, TestSource>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<IVisitorRepository, VisitorRepository>();
            services.AddScoped<CurrencyService, CurrencyService>();
        }
    }
}