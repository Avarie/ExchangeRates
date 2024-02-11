using ExchangeRates.Service;
using ExchangeRates.Sources;
using ExchangeRates.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace ExchangeRates
{
    public static class StartupExtensions
    {
        public static void RegisterImplementations(this IServiceCollection services)
        {
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<IVisitorRepository, VisitorRepository>();
            services.AddScoped<CurrencyService, CurrencyService>();
        }

        public static void RegisterSources(this IServiceCollection services)
        {
            services.AddScoped<ICurrencyDataSource, ObmenkaKhSource>();
            services.AddScoped<ICurrencyDataSource, PrivatBankSource>();
            services.AddScoped<ICurrencyDataSource, NbuBankSource>();
            services.AddScoped<ICurrencyDataSource, MonoBankSource>();

            // disabled due to continuous changes in their html structure
            // TODO: review it later
            // services.AddScoped<ICurrencyDataSource, InvestingComSource>();
        }
    }
}