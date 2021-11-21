using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CurrencyAggregator.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CurrencyAggregator.Models;
using CurrencyAggregator.Resources;
using CurrencyAggregator.Service;
using CurrencyAggregator.Sources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using CurrencyAggregator.Repository;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CurrencyAggregator.Controllers
{
    public class DataController : Controller
    {
        private readonly ILogger<DataController> _logger;
        private readonly CurrencyService _service;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IVisitorRepository _visitorRepository;

        public DataController(ILogger<DataController> logger, CurrencyService service, ICurrencyRepository currencyRepository, IVisitorRepository visitorRepository)
        {
            _logger = logger;
            _service = service;
            _currencyRepository = currencyRepository;
            _visitorRepository = visitorRepository;
        }
        
        public ChartResult GetData([FromBody] object data)
        {
            var request = JsonConvert.DeserializeObject<RequestParams>(JsonSerializer.Serialize(data));

            var aggregated = _service.AggregateData(request.Range, request.Currencies.ToArray());

            var assets = new List<ChartItem>();

            foreach (var operation in request.Operations.Select(WebParameters.GetOperation))
            {
                foreach (var currency in request.Currencies)
                {
                    assets.AddRange(ChartHelper.ChartDataSets(aggregated, currency, request, operation));
                }
            }

            ChartHelper.FillMissedData(assets, aggregated.Dates);

            return new ChartResult(Str.ChartTitle, aggregated.Dates.Select(request.FormatDate).ToList(), assets);

        }

        public async Task<object> GetVolatility()
        {
            return new
            {
                title = Str.VolatilityTitle,
                labels = (await _currencyRepository.GetPreparedAsync(PreparedDataItemKeys.VolatilityLabels)).Data,
                datasets = (await _currencyRepository.GetPreparedAsync(PreparedDataItemKeys.VolatilityDataSets)).Data,
                percents = (await _currencyRepository.GetPreparedAsync(PreparedDataItemKeys.VolatilityPercents)).Data
            };
        }

        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
        
        public async Task<object> GerFinDi([FromBody] Dictionary<string, FingerPrintData> gerfin)
        {
            var ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();

            var result = _visitorRepository.Find(gerfin);

            _visitorRepository.CreateOrUpdate(result, gerfin, ip);

            _visitorRepository.Save();

            return _visitorRepository.GetAll().Select(x => x.LastVisit.Value.Date)
                .Count(x => x == DateTime.Today);
        }

        public IActionResult Index() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}