using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeRates.Helpers;
using ExchangeRates.Models;
using ExchangeRates.Resources;

namespace ExchangeRates.Sources
{
    public static class CurrencyTypes
    {
        public const string USD = "USD";
        public const string EUR = "EUR";
        public const string CNY = "CNY";
        public const string GBP = "GBP";

        public static string[] Types => new[] {USD, EUR, GBP, CNY};

        public static string GetVolatilityColor(string currency) => currency switch
        {
            USD => "green",
            EUR => "blue",
            GBP => "red",
            CNY => "gold"
        };
    }
    
    public static class Operation
    {
        public const string Buy = "BUY";
        public const string Sell = "SELL";
        public const string Diff = "DIFF";

        public static string GetTitle(string val) => val switch
        {
            Buy => Str.Operation_Buy_Title,
            Sell => Str.Operation_Sell_Title,
            Diff => Str.Operation_Diff_Title
        };
    }

    public class SettingSet
    {
        public string Key { get; set; }

        /// <summary>
        /// Color scheme
        /// </summary>
        public ChartColor Color { get; set; }

        /// <summary>
        /// Title on the page
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Title on the page
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// By default on the page
        /// </summary>
        public bool Enabled { get; set; }
        public Func<CurrencyItem, decimal> ValueFunc { get; set; }
    }

    public static class WebParameters
    {
        public static SettingSet[] Currencies => new SettingSet[]
        {
            new SettingSet {Key = CurrencyTypes.USD, Title = Str.USD_Title, Description = Str.USD_Desc, Enabled = true}, 
            new SettingSet {Key = CurrencyTypes.EUR, Title = Str.EUR_Title, Description = Str.EUR_Desc, Enabled = false}, 
            new SettingSet {Key = CurrencyTypes.GBP, Title = Str.GBP_Title, Description = Str.GBP_Desc, Enabled = false}, 
            new SettingSet {Key = CurrencyTypes.CNY, Title = Str.CNY_Title, Description = Str.CNY_Desc, Enabled = false}, 
        };
        
        public static SettingSet[] Operations => new SettingSet[]
        {
            new SettingSet {Key = Operation.Buy, Color = ChartColor.Green, Title = Str.Operation_Buy_Title, Description = Str.Operation_Buy_Desc, Enabled = true, ValueFunc = (x) => x.Buy}, 
            new SettingSet {Key = Operation.Sell, Color = ChartColor.Red, Title = Str.Operation_Sell_Title, Description = Str.Operation_Sell_Desc, Enabled = false, ValueFunc = (x) => x.Sell},
            new SettingSet {Key = Operation.Diff, Color = ChartColor.Red, Title = Str.Operation_Diff_Title, Description = Str.Operation_Diff_Desc, Enabled = false, ValueFunc = (x) => x.Diff},
        };
        
        public static KeyValuePair<string, string>[] Range => new[]
        {
            new KeyValuePair<string, string>("D", Str.Range_Day), 
            new KeyValuePair<string, string>("T", Str.Range_ThreeDays), 
            new KeyValuePair<string, string>("W", Str.Range_Week), 
            new KeyValuePair<string, string>("M", Str.Range_Month), 
            new KeyValuePair<string, string>("Y", Str.Range_Year), 
            new KeyValuePair<string, string>("A", Str.Range_All)
        };

        public static SettingSet GetOperation(string p) => Operations.First(x => x.Key == p);
        public static string GetCurrencyTitle(string p) => Currencies.First(x => x.Key == p).Title;
    }
}
