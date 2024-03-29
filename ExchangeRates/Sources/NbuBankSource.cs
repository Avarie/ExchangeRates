﻿// https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json

using RestSharp;
using System.Net;
using System.Linq;
using Newtonsoft.Json;
using ExchangeRates.Models;
using System.Collections.Generic;

namespace ExchangeRates.Sources
{
    public class NbuBankSource : ICurrencyDataSource
    {
        private class NbuItem
        {
            [JsonProperty("r030")]
            public long R030 { get; set; }

            [JsonProperty("txt")]
            public string Name { get; set; }

            [JsonProperty("rate")]
            public decimal? Rate { get; set; }

            [JsonProperty("cc")]
            public string CurrencyCode { get; set; }

            [JsonProperty("exchangedate")]
            public string Date { get; set; }
        }

        public List<CurrencyItem> GetSource()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            var client = new RestClient("https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json");
            var response = client.Execute(new RestRequest());

            var result = JsonConvert.DeserializeObject<NbuItem[]>(response.Content);

            return result.Where(x => CurrencyTypes.RelatedCurrencyTypes.Contains(x.CurrencyCode) && x.Rate.HasValue)
                .Select(x => new CurrencyItem(Helpers.SourceListHelper.NBU) { Type = x.CurrencyCode, Buy = x.Rate.Value }).ToList();
        }
    }

    // [
    //     {
    //     "r030":36,"txt":"Австралійський долар","rate":17.4634,"cc":"AUD","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":124,"txt":"Канадський долар","rate":19.0989,"cc":"CAD","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":156,"txt":"Юань Женьмiньбi","rate":3.7496,"cc":"CNY","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":191,"txt":"Куна","rate":3.845,"cc":"HRK","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":203,"txt":"Чеська крона","rate":1.0694,"cc":"CZK","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":208,"txt":"Данська крона","rate":3.9111,"cc":"DKK","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":344,"txt":"Гонконгівський долар","rate":3.4497,"cc":"HKD","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":348,"txt":"Форинт","rate":0.083483,"cc":"HUF","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":356,"txt":"Індійська рупія","rate":0.35228,"cc":"INR","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":360,"txt":"Рупія","rate":0.0018184,"cc":"IDR","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":376,"txt":"Новий ізраїльський шекель","rate":7.5842,"cc":"ILS","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":392,"txt":"Єна","rate":0.24881,"cc":"JPY","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":398,"txt":"Теньге","rate":0.064363,"cc":"KZT","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":410,"txt":"Вона","rate":0.021634,"cc":"KRW","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":484,"txt":"Мексиканське песо","rate":1.1638,"cc":"MXN","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":498,"txt":"Молдовський лей","rate":1.507,"cc":"MDL","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":554,"txt":"Новозеландський долар","rate":16.3196,"cc":"NZD","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":578,"txt":"Норвезька крона","rate":2.6733,"cc":"NOK","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":643,"txt":"Російський рубль","rate":0.37425,"cc":"RUB","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":682,"txt":"Саудівський рiял","rate":7.1198,"cc":"SAR","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":702,"txt":"Сінгапурський долар","rate":18.789,"cc":"SGD","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":710,"txt":"Ренд","rate":1.5139,"cc":"ZAR","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":752,"txt":"Шведська крона","rate":2.7671,"cc":"SEK","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":756,"txt":"Швейцарський франк","rate":27.5476,"cc":"CHF","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":818,"txt":"Єгипетський фунт","rate":1.6869,"cc":"EGP","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":826,"txt":"Фунт стерлінгів","rate":32.5723,"cc":"GBP","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":840,"txt":"Долар США","rate":26.7556,"cc":"USD","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":933,"txt":"Бiлоруський рубль","rate":11.1686,"cc":"BYN","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":944,"txt":"Азербайджанський манат","rate":15.697,"cc":"AZN","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":946,"txt":"Румунський лей","rate":6.0221,"cc":"RON","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":949,"txt":"Турецька ліра","rate":3.9307,"cc":"TRY","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":960,"txt":"СПЗ (спеціальні права запозичення)","rate":36.4403,"cc":"XDR","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":975,"txt":"Болгарський лев","rate":14.9148,"cc":"BGN","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":978,"txt":"Євро","rate":29.1676,"cc":"EUR","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":985,"txt":"Злотий","rate":6.4527,"cc":"PLN","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":12,"txt":"Алжирський динар","rate":0.21003,"cc":"DZD","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":50,"txt":"Така","rate":0.31735,"cc":"BDT","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":51,"txt":"Вірменський драм","rate":0.056249,"cc":"AMD","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":364,"txt":"Іранський ріал","rate":0.00064189,"cc":"IRR","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":368,"txt":"Іракський динар","rate":0.022655,"cc":"IQD","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":417,"txt":"Сом","rate":0.34174,"cc":"KGS","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":422,"txt":"Ліванський фунт","rate":0.017883,"cc":"LBP","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":434,"txt":"Лівійський динар","rate":19.0551,"cc":"LYD","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":458,"txt":"Малайзійський ринггіт","rate":6.2646,"cc":"MYR","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":504,"txt":"Марокканський дирхам","rate":2.7221,"cc":"MAD","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":704,"txt":"Донг","rate":0.0011567,"cc":"VND","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":764,"txt":"Бат","rate":0.83249,"cc":"THB","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":784,"txt":"Дирхам ОАЕ","rate":7.3398,"cc":"AED","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":788,"txt":"Туніський динар","rate":9.2895,"cc":"TND","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":860,"txt":"Узбецький сум","rate":0.0026613,"cc":"UZS","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":901,"txt":"Новий тайванський долар","rate":0.90698,"cc":"TWD","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":934,"txt":"Туркменський новий манат","rate":7.7026,"cc":"TMT","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":936,"txt":"Ганське седі","rate":4.6473,"cc":"GHS","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":941,"txt":"Сербський динар","rate":0.24908,"cc":"RSD","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":972,"txt":"Сомоні","rate":2.6325,"cc":"TJS","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":981,"txt":"Ларі","rate":8.4182,"cc":"GEL","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":986,"txt":"Бразильський реал","rate":4.9977,"cc":"BRL","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":959,"txt":"Золото","rate":46477.96,"cc":"XAU","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":961,"txt":"Срiбло","rate":458.63,"cc":"XAG","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":962,"txt":"Платина","rate":22436.71,"cc":"XPT","exchangedate":"25.05.2020"
    //      }
    //     ,{
    //     "r030":964,"txt":"Паладiй","rate":53084.18,"cc":"XPD","exchangedate":"25.05.2020"
    //      }
    // ]
}