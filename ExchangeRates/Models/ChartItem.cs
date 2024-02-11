using System.Collections.Generic;

namespace ExchangeRates.Models
{
    public class ChartItem
    {
        public string label { get; set; }
        public string borderColor { get; set; }
        public string backgroundColor { get; set; }
        public bool fill { get; set; }
        public List<decimal> data { get; set; }
        public string yAxisID { get; set; }
    }
}