using System;
using System.Net;
using System.Linq;
using AngleSharp.Dom;
using ExchangeRates.Models;
using ExchangeRates.Helpers;
using AngleSharp.Html.Parser;
using System.Collections.Generic;

namespace ExchangeRates.Sources
{
    public class ObmenkaKhSource : ICurrencyDataSource
    {
        private List<IElement> _elements = new List<IElement>();

        public List<CurrencyItem> GetSource()
        {
            string doc = null;

            using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
            {
                // Or you can get the file content without saving it
                doc = client.DownloadString("http://obmenka.kh.ua/");
            }

            _elements = new HtmlParser().ParseDocument(doc).All.Where(m => m.LocalName == "span").ToList();

            return new List<CurrencyItem>
            {
                GetItemByCurrencyType(SourceListHelper.ObmenkaRetail, CurrencyTypes.USD, "usd_buy", "usd_sale"),
                GetItemByCurrencyType(SourceListHelper.ObmenkaWholesale, CurrencyTypes.USD, "usd_buy_wsale", "usd_sale_wsale"),

                GetItemByCurrencyType(SourceListHelper.ObmenkaRetail, CurrencyTypes.EUR, "eur_buy", "eur_sale"),
                GetItemByCurrencyType(SourceListHelper.ObmenkaWholesale, CurrencyTypes.EUR, "eur_buy_wsale", "eur_sale_wsale"),

                GetItemByCurrencyType(SourceListHelper.ObmenkaRetail, CurrencyTypes.GBP, "gbp_buy", "gbp_sale"),
                GetItemByCurrencyType(SourceListHelper.ObmenkaWholesale, CurrencyTypes.GBP, "gbp_buy_wsale", "gbp_sale_wsale"),
            };
        }

        private CurrencyItem GetItemByCurrencyType(string name, string type, string buyId, string sellId)
        {
            return new CurrencyItem(name)
            {
                Buy = GetData("buy", buyId),
                Sell = GetData("sell", sellId),
                Type = type
            };
        }

        private decimal GetData(string className, string name)
        {
            try
            {
                var element = _elements.Find(s => s.ClassName == className && s.OuterHtml.Contains($"\"{name}\""))?.Text();
                return Convert.ToDecimal(element);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Parser exception: {className}/{name}", ex);
            }
        }
    }
}