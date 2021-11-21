using System;
using System.Collections.Generic;

namespace CurrencyAggregator.Models
{
    public class ChartResult
    {
        public ChartResult(string t, List<string> l, List<ChartItem> d)
        {
            title = t;
            labels = l;
            datasets = d;
        }

        public string title { get; set; }
        public List<string> labels { get; set; }
        public List<ChartItem> datasets { get; set; }
    }
}
