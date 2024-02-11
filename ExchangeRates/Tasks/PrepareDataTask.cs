using Hangfire;
using System.Linq;
using ExchangeRates.Models;
using ExchangeRates.Helpers;
using ExchangeRates.Service;
using ExchangeRates.Sources;
using ExchangeRates.Repository;
using Range = ExchangeRates.Models.Range;

namespace ExchangeRates.Tasks
{
    public class PrepareDataTask
    {
        private readonly ICurrencyRepository _repo;
        private readonly CurrencyService _service;

        public PrepareDataTask(ICurrencyRepository currencyRepository, CurrencyService service)
        {
            _repo = currencyRepository;
            _service = service;
        }

        public void Run(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var aggregated = _service.AggregateData(Range.M, CurrencyTypes.RelatedCurrencyTypes);
            var assets = VolatilityHelper.CalculateVolatility(aggregated);

            _repo.CleanPrepared();
            _repo.SavePrepared(PreparedDataItemKeys.VolatilityLabels, aggregated.Dates.Select(x => x.ToString("dd-MM-yy")).ToList());
            _repo.SavePrepared(PreparedDataItemKeys.VolatilityDataSets, assets);
            _repo.SavePrepared(PreparedDataItemKeys.VolatilityPercents, assets.Select(x => new { id = x.label, title = WebParameters.GetCurrencyTitle(x.label), value = VolatilityHelper.CalculateVolatilityPercentage(x) }));
            _repo.Save();
        }
    }
}