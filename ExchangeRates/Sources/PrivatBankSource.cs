// Наличный курс ПриватБанка (в отделениях):
//
// GET XML: https://api.privatbank.ua/p24api/pubinfo?exchange&coursid=5
// GET JSON: https://api.privatbank.ua/p24api/pubinfo?json&exchange&coursid=5
// Безналичный курс ПриватБанка (конвертация по картам, Приват24, пополнение вкладов):
//
// GET XML: https://api.privatbank.ua/p24api/pubinfo?exchange&coursid=11
// GET JSON: https://api.privatbank.ua/p24api/pubinfo?exchange&json&coursid=11

using RestSharp;
using System.Net;
using System.Linq;
using Newtonsoft.Json;
using ExchangeRates.Models;
using System.Collections.Generic;

namespace ExchangeRates.Sources
{
    public class PrivatBankSource : ICurrencyDataSource
    {
        private class PrivatItem
        {
            [JsonProperty("ccy")]
            public string CurrencyCode { get; set; }

            [JsonProperty("base_ccy")]
            public string BaseCurrencyCode { get; set; }

            [JsonProperty("buy")]
            public decimal Buy { get; set; }

            [JsonProperty("sale")]
            public decimal Sale { get; set; }
        }

        public List<CurrencyItem> GetSource()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            var items = CurrencyItems(Helpers.SourceListHelper.PrivatBankCash, "https://api.privatbank.ua/p24api/pubinfo?json&exchange&coursid=5");
            var cashLessItems = CurrencyItems(Helpers.SourceListHelper.PrivatBankCashLess, "https://api.privatbank.ua/p24api/pubinfo?exchange&json&coursid=11");

            items.AddRange(cashLessItems);
            return items;
        }

        private static List<CurrencyItem> CurrencyItems(string name, string url)
        {
            var client = new RestClient(url);
            var response = client.Execute(new RestRequest());

            var result = JsonConvert.DeserializeObject<PrivatItem[]>(response.Content);

            return result.Where(x => CurrencyTypes.RelatedCurrencyTypes.Contains(x.CurrencyCode))
                .Select(x => new CurrencyItem(name) { Type = x.CurrencyCode, Buy = x.Buy, Sell = x.Sale }).ToList();
        }
    }
}