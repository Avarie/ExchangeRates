using System;
using Serilog;
using Hangfire;
using ExchangeRates.Models;
using ExchangeRates.Sources;
using ExchangeRates.Repository;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace ExchangeRates.Tasks
{
    public class GetSourceDataTask
    {
        private readonly IServiceProvider _ioc;
        private readonly ICurrencyRepository _currencyRepository;

        public GetSourceDataTask(IServiceProvider ioc, ICurrencyRepository currencyRepository)
        {
            _ioc = ioc;
            _currencyRepository = currencyRepository;
        }

        public void Run(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            Log.Information("GetSourceDataTask is started.");

            var sources = _ioc.GetServices<ICurrencyDataSource>();
            var result = new List<CurrencyItem>();

            foreach (var source in sources)
            {
                Log.Information($"   ... process source: {source}");
                try
                {
                    result.AddRange(source.GetSource());
                }
                catch (Exception ex)
                {
                    Log.Error($"   ... failed: {ex.Message}");
                }
            }

            result.ForEach(x => _currencyRepository.Add(x));

            _currencyRepository.Save();

            Log.Information("GetSourceDataTask is completed.");
        }
    }
}