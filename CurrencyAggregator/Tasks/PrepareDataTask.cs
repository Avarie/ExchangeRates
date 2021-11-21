using System;
using System.Linq;
using CurrencyAggregator.Controllers;
using CurrencyAggregator.Helpers;
using CurrencyAggregator.Models;
using CurrencyAggregator.Repository;
using CurrencyAggregator.Service;
using CurrencyAggregator.Sources;
using Hangfire;
using Microsoft.Extensions.Logging;
using Range = CurrencyAggregator.Models.Range;

namespace CurrencyAggregator.Tasks
{
    public class PrepareDataTask
    {
        private readonly ILogger<DataController> _logger;
        private readonly ICurrencyRepository _repo;
        private readonly CurrencyService _service;
        public PrepareDataTask(ILogger<DataController> logger, ICurrencyRepository currencyRepository, CurrencyService service)
        {
            _logger = logger;
            _repo = currencyRepository;
            _service = service;
        }

        public void Run(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var aggregated = _service.AggregateData(Range.M, CurrencyTypes.Types);
            var assets = VolatilityHelper.CalculateVolatility(aggregated);

            _repo.CleanPrepared();
            _repo.SavePrepared(PreparedDataItemKeys.VolatilityLabels, aggregated.Dates.Select(x => x.ToString("dd-MM-yy")).ToList());
            _repo.SavePrepared(PreparedDataItemKeys.VolatilityDataSets, assets);
            _repo.SavePrepared(PreparedDataItemKeys.VolatilityPercents, assets.Select(x => new { id = x.label, title = WebParameters.GetCurrencyTitle(x.label), value = VolatilityHelper.CalculateVolatilityPercentage(x) }));
            _repo.Save();

            _logger.LogDebug("Prepared data is saved !!!1");
        }
    }
}