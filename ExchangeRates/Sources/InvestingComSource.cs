using System;
using RestSharp;
using System.Net;
using System.Linq;
using AngleSharp.Dom;
using ExchangeRates.Models;
using ExchangeRates.Helpers;
using AngleSharp.Html.Parser;
using System.Collections.Generic;

namespace ExchangeRates.Sources
{
    public class InvestingComSource : ICurrencyDataSource
    {
        public InvestingComSource()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
        }

        public List<CurrencyItem> GetSource()
        {
            var usd = GetSource(CurrencyTypes.USD, "https://www.investing.com/currencies/usd-uah-chart");
            var eur = GetSource(CurrencyTypes.EUR, "https://www.investing.com/currencies/eur-uah-chart");
            var gbp = GetSource(CurrencyTypes.GBP, "https://www.investing.com/currencies/gbp-uah-chart");
            var cny = GetSource(CurrencyTypes.CNY, "https://www.investing.com/currencies/cny-uah-chart");

            return new List<CurrencyItem>() { usd, eur, gbp, cny };
        }

        private CurrencyItem GetSource(string type, string link)
        {
            var request = new RestRequest("/", Method.Get);
            var client = new RestClient(link);
            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK) throw new AggregateException("Unable to download file");

            var investData = new HtmlParser().ParseDocument(response.Content).All
                .Where(m =>
                            m.LocalName == "div" &&
                            m.OuterHtml.Contains("data-test=\"bid-value\"") &&
                            m.InnerHtml.Contains("class=\"px-1") &&
                            m.Children.All(x => x.LocalName == "span") &&
                            m.Children.Count() == 3
                )
                .ToList();

            if (investData.Count is not 1) throw new AggregateException("Unable to parse data!");

            var childs = investData[0].Children;

            return
                new CurrencyItem(SourceListHelper.InvestingCom)
                {
                    Buy = GetData(childs.First()),
                    Sell = GetData(childs.Last()),
                    Type = type
                };
        }

        private decimal GetData(IElement element)
        {
            if (string.IsNullOrEmpty(element.Text())) return 0;
            return Convert.ToDecimal(element.Text());
        }
    }
}