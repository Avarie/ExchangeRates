using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExchangeRates.Controllers;
using ExchangeRates.Models;
using ExchangeRates.Repository;
using ExchangeRates.Sources;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ExchangeRates.Tasks
{
    public class GetSourceDataTask
    {
        private readonly ILogger<DataController> _logger;
        private readonly IServiceProvider _ioc;
        private readonly ICurrencyRepository _currencyRepository;
        public GetSourceDataTask(ILogger<DataController> logger, IServiceProvider ioc, ICurrencyRepository currencyRepository)
        {
            _logger = logger;
            _ioc = ioc;
            _currencyRepository = currencyRepository;
        }

        public void Run(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            Process(DateTime.Now);
        }

        public void Process(DateTime now)
        {
            _logger.LogDebug("Hangfire Task is started !!!");
            var sources = _ioc.GetServices<ICurrencyDataSource>();
            var result = new List<CurrencyItem>();

            foreach (var source in sources)
            {
                _logger.LogDebug($"   ... process source: {source}");
                try
                {
                    result.AddRange(source.GetSource());
                }
                catch (Exception ex)
                {
                    _logger.LogDebug($"   ... failed: {ex.Message}");
                }
            }

            result.ForEach(x => _currencyRepository.Add(x));
            _currencyRepository.Save();
            _logger.LogDebug("Hangfire Task is completed !!!");
        }
    }
}