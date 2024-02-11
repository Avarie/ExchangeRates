using System;
using System.Collections.Generic;

namespace ExchangeRates.Models
{
    internal class AggregateSet
    {
        public AggregateSet(DateTime startDate, DateTime maxDate, Func<DateTime, DateTime> addDateDiff, Range range, Func<List<CurrencyItem>, CurrencyItem> calcCurrencyItemFunc)
        {
            StartDate = startDate;
            MaxDate = maxDate;
            AddDateDiff = addDateDiff;
            DateFormat = GetDateFormatFunc(range);
            CalcCurrencyItemFunc = calcCurrencyItemFunc;
        }

        public DateTime StartDate { get; }
        public DateTime MaxDate { get; }
        public Func<DateTime, DateTime> AddDateDiff { get; }
        public Func<DateTime, DateTime> DateFormat { get; }
        public Func<List<CurrencyItem>, CurrencyItem> CalcCurrencyItemFunc { get; }

        private Func<DateTime, DateTime> GetDateFormatFunc(Range range)
        {
            var hourFn = new Func<DateTime, DateTime>(dt => new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0));
            var dayFn = new Func<DateTime, DateTime>(dt => new DateTime(dt.Year, dt.Month, dt.Day, 12, 0, 0));
            var monthFn = new Func<DateTime, DateTime>(dt => new DateTime(dt.Year, dt.Month, 1, 12, 0, 0));

            return range switch
            {
                Range.D => hourFn,
                Range.T => hourFn,
                Range.W => dayFn,
                Range.M => dayFn,
                Range.Y => dayFn,
                _ => monthFn
            };
        }
    }
}