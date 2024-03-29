﻿using System;
using System.Linq;
using ExchangeRates.Models;
using ExchangeRates.Sources;
using ExchangeRates.Repository;
using System.Collections.Generic;
using Range = ExchangeRates.Models.Range;

namespace ExchangeRates.Service
{
    public class CurrencyService
    {
        private readonly ICurrencyRepository _repository;

        public CurrencyService(ICurrencyRepository repository)
        {
            _repository = repository;
        }

        public IQueryable<CurrencyItem> GetAll(Range range)
        {
            var fromDate = range switch
            {
                Range.D => DateTime.Now.AddDays(-1),
                Range.T => DateTime.Now.AddDays(-3),
                Range.W => DateTime.Now.AddDays(-7),
                Range.M => DateTime.Now.AddMonths(-1),
                Range.Y => DateTime.Now.AddMonths(-12),
                _ => DateTime.MinValue,
            };

            return _repository.GetAll().Where(x => x.Date >= fromDate);
        }

        public AggregatedDataResult AggregateData(Range range, string[] currencyTypes)
        {
            var set = GetAll(range);
            currencyTypes ??= CurrencyTypes.RelatedCurrencyTypes;

            // in case if db is outdated and doesn't have related data for the selected range
            if (!set.Any()) return new AggregatedDataResult();

            var min = set.Min(x => x.Date);
            var max = set.Max(x => x.Date);

            var minDate = new DateTime(min.Year, min.Month, min.Day, min.Hour, 0, 0);
            var maxDate = new DateTime(max.Year, max.Month, max.Day, max.Hour, 0, 0);

            var minDateMonth = new DateTime(min.Year, min.Month, 1);
            var maxDateMonth = new DateTime(max.Year, max.Month, 1);

            var lastItemFn = new Func<List<CurrencyItem>, CurrencyItem>(items =>
            {
                var last = items.LastOrDefault();
                return new CurrencyItem
                {
                    Buy = last?.Buy ?? 0,
                    Sell = last?.Sell ?? 0
                };
            });

            var averageItemsFn = new Func<List<CurrencyItem>, CurrencyItem>(items =>
             new CurrencyItem
             {
                 Buy = items.Average(x => x.Buy),
                 Sell = items.Average(x => x.Sell)
             }
            );

            var settings = range switch
            {
                Range.D => new AggregateSet(minDate, maxDate, dt => dt.AddHours(1), range, lastItemFn),
                Range.T => new AggregateSet(minDate, maxDate, dt => dt.AddHours(1), range, lastItemFn),
                Range.W => new AggregateSet(minDate, maxDate, dt => dt.AddDays(1), range, lastItemFn),
                Range.M => new AggregateSet(minDate, maxDate, dt => dt.AddDays(1), range, lastItemFn),
                Range.Y => new AggregateSet(minDate, maxDate, dt => dt.AddDays(7), range, averageItemsFn),
                _ => new AggregateSet(minDateMonth, maxDateMonth, dt => dt.AddMonths(1), range, averageItemsFn)
            };

            var result = AggregateData(set, currencyTypes, settings);

            result.SourceNames = result.SourceNames.Distinct().ToList();

            return result;
        }

        private AggregatedDataResult AggregateData(IQueryable<CurrencyItem> set, string[] currencyTypes, AggregateSet s)
        {
            var result = new AggregatedDataResult();

            for (var date = s.StartDate; date <= s.MaxDate; date = s.AddDateDiff(date))
            {
                var dateToStore = s.DateFormat(date);
                var rangeSet = set.Where(x => x.Date >= date && x.Date < s.AddDateDiff(date));

                foreach (var currencyType in currencyTypes)
                {
                    var currencyRangeSet = rangeSet.Where(x => x.Type == currencyType).ToList();
                    var sourceNames = currencyRangeSet.Select(x => x.Name).Distinct();

                    foreach (var source in sourceNames)
                    {
                        var sourceSet = currencyRangeSet.Where(x => x.Name == source).OrderBy(x => x.Date).ToList();

                        var calculated = s.CalcCurrencyItemFunc(sourceSet);

                        result.Items.Add(new CurrencyItem(source)
                        {
                            Date = dateToStore,
                            Type = currencyType,
                            Buy = calculated.Buy,
                            Sell = calculated.Sell
                        });
                        result.SourceNames.Add(source);
                    }
                }

                result.Dates.Add(dateToStore);
            }

            return result;
        }
    }
}