using RestSharp;
using System.Net;
using System.Linq;
using Newtonsoft.Json;
using ExchangeRates.Models;
using System.Collections.Generic;

namespace ExchangeRates.Sources
{
    /// <summary>
    /// https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json
    /// </summary>
    public class MonoBankSource : ICurrencyDataSource
    {
        private class MonoBankItem
        {
            public int currencyCodeA { get; set; }
            public int currencyCodeB { get; set; }
            public int date { get; set; }
            public decimal? rateBuy { get; set; }
            public decimal? rateSell { get; set; }
            public decimal? rateCross { get; set; }
        }

        public List<CurrencyItem> GetSource()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            var client = new RestClient("https://api.monobank.ua/bank/currency");
            var response = client.Execute(new RestRequest());

            var result = JsonConvert.DeserializeObject<MonoBankItem[]>(response.Content);

            result = result
                .Where(x =>
                        x.currencyCodeB == CurrencyCodes.UAH &&
                        CurrencyCodes.RelatedCurrencyTypes.Contains(x.currencyCodeA) &&
                        x.rateBuy is not null && x.rateSell is not null
                        )
                .ToArray();

            return result
                .Select(x => new CurrencyItem(Helpers.SourceListHelper.MonoBank)
                {
                    Type = CurrencyCodes.GetTypeByCode(x.currencyCodeA),
                    Buy = x.rateBuy.Value,
                    Sell = x.rateSell.Value
                }
                ).ToList();
        }
    }
}