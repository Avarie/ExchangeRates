using System;
using System.Collections.Generic;

namespace CurrencyAggregator.Models
{
    public class AggregatedDataResult
    {
        public List<CurrencyItem> Items { get; set; } = new List<CurrencyItem>();
        public List<DateTime> Dates { get; set; } = new List<DateTime>();
        public List<string> SourceNames { get; set; } = new List<string>();
    }
}
