using System;
using System.Collections.Generic;

namespace ExchangeRates.Models
{
    public class AggregatedDataResult
    {
        public List<CurrencyItem> Items { get; set; } = [];
        public List<DateTime> Dates { get; set; } = [];
        public List<string> SourceNames { get; set; } = [];
    }
}