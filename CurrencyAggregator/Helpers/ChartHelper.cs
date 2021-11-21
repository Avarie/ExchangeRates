using System;
using System.Collections.Generic;
using System.Linq;
using CurrencyAggregator.Models;
using CurrencyAggregator.Sources;

namespace CurrencyAggregator.Helpers
{
    public static class ChartHelper
    {
        public static List<ChartItem> ChartDataSets(AggregatedDataResult data, string currency, RequestParams request, SettingSet parameter)
        {
            var set = data.Items.Where(x => x.Type == currency);
            var res = new List<KeyValuePair<string, List<decimal>>>();
            var sources = data.SourceNames.OrderBy(x => x);

            foreach (var source in sources)
            {
                var sourceItems = set.Where(x => x.Name == source).ToList();
                var prices = new List<decimal>();

                // avoid unused source parts like NBU Sell. NBU SELL is always null for us.
                if (!sourceItems.Any() || (parameter.Key != Operation.Buy && sourceItems.All(x => x.Sell == 0))) continue;

                foreach (var date in data.Dates)
                {
                    CurrencyItem priceItem;

                    if (request.ShouldDateBeHourly())
                    {
                        var dt = new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0);
                        priceItem = sourceItems.Where(x => new DateTime(x.Date.Year, x.Date.Month, x.Date.Day, x.Date.Hour, 0, 0) == dt)
                            .OrderByDescending(x => x.Date)
                            .Distinct()
                            .FirstOrDefault();
                    }
                    else
                    {
                        priceItem = sourceItems.Where(x => x.Date.Date == date.Date)
                            .OrderByDescending(x => x.Date)
                            .FirstOrDefault();
                    }

                    if (priceItem == null) continue;

                    prices.Add(parameter.ValueFunc(priceItem));
                }

                res.Add(new KeyValuePair<string, List<decimal>>(source, prices));
            }

            var htmlColorSelector = 0;
            var dataSets = res.Select(x => new ChartItem
            {
                label = request.Operations.Count > 1 ? $"[{Operation.GetTitle(parameter.Key)}] {SourceListHelper.GetTitle(x.Key)}" : $"{SourceListHelper.GetTitle(x.Key)}",
                borderColor = ColorHelper.GetColor(parameter.Color, htmlColorSelector),
                backgroundColor = ColorHelper.GetColor(parameter.Color, htmlColorSelector++),
                fill = false,
                data = x.Value,
                yAxisID = "y-axis-1"
            });

            return dataSets.ToList();
        }

        public static void FillMissedData(List<ChartItem> datasets, List<DateTime> dates)
        {
            foreach (var chartItem in datasets)
            {
                var missedNumber = dates.Count - chartItem.data.Count;
                if (missedNumber == 0) continue;
                if (missedNumber < 0) throw new ArgumentOutOfRangeException("Something is wrong with number of sets");

                var initialValue = chartItem.data.FirstOrDefault();
                //var missedElements = new decimal[missedNumber];
                var D = new List<decimal>();
                for (int i = 0; i < missedNumber; i++) D.Add(initialValue);
                var oldElem = chartItem.data.ToList();
                chartItem.data.Clear();
                chartItem.data.AddRange(D);
                chartItem.data.AddRange(oldElem);
            }
        }
    }
}
