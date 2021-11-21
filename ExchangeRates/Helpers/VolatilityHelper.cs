using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeRates.Models;
using ExchangeRates.Sources;

namespace ExchangeRates.Helpers
{
    public static class VolatilityHelper
    {
        public static decimal CalculateVolatilityPercentage(ChartItem set)
        {
            var max = set.data.Max() * 1.05m;
            var value = set.data.Last();

            if (max == 0) max = 0.01m;
            return value * 100 / max;
        }

        public static List<ChartItem> CalculateVolatility(AggregatedDataResult data)
        {
            var result = new List<ChartItem>();

            foreach (var currency in CurrencyTypes.Types)
            {
                var dataSets = data.Items.Where(x => x.Type == currency).ToList();
                var averageParam = dataSets.SelectMany(x => new[]{ x.Buy, x.Sell }).Average();
                var count = data.Dates.Count;

                var voCommonAverage = new List<decimal>();

                foreach (var date in data.Dates)
                {
                    var dataasets = dataSets.Where(x => x.Date.Date == date.Date).ToList();

                    if (!dataasets.Any())
                    {
                        var cal = voCommonAverage.Any() ? voCommonAverage.Last() : 0;
                        voCommonAverage.Add(cal);
                    }
                    else
                    {
                        var cal0 = dataasets
                            .SelectMany(x => new[] { x.Buy, x.Sell })
                            .Average();

                        var cal = CalcVolCurrent(cal0, averageParam, count);
                        voCommonAverage.Add(cal);
                    }
                }

                //transform to percent
                var max = voCommonAverage.Max();
                if (max > 0)
                {
                    var voPercents = new List<decimal>();
                    foreach (var val in voCommonAverage)
                    {
                        voPercents.Add(val * 100 / max);
                    }

                    voCommonAverage.Clear();
                    voCommonAverage.AddRange(voPercents);
                }

                result.Add(AddCalculateVolatilityChart(voCommonAverage, currency, CurrencyTypes.GetVolatilityColor(currency)));
            }

            return result;
        }

        private static decimal CalcVolCurrent(decimal current, decimal average, int count) => (decimal)Math.Sqrt(Math.Pow((double)(current - average), 2) / count);
        private static ChartItem AddCalculateVolatilityChart(List<decimal> list, string label, string color)
        => new ChartItem
            {
                label = label,
                borderColor = color,
                backgroundColor = color,
                fill = false,
                data = list,
                yAxisID = "y-axis-1"
            };
    }
}
