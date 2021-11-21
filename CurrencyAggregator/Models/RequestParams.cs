using System;
using System.Collections.Generic;

namespace CurrencyAggregator.Models
{
    public class RequestParams
    {
        public List<string> Currencies { get; set; } = new List<string>();
        public List<string> Operations { get; set; } = new List<string>();

        public Range Range { get; set; }
        public bool ShouldDateBeHourly() => Range == Range.T || Range == Range.D;
        public string FormatDate(DateTime x) => ShouldDateBeHourly() ? x.ToString("dd-MM HH:mm") : x.ToString("dd-MM-yy");
    }

    public enum Range
    {
        /// <summary>
        /// 1 Day
        /// </summary>
        D, 
        /// <summary>
        /// Three days
        /// </summary>
        T, 
        /// <summary>
        /// One week
        /// </summary>
        W, 
        /// <summary>
        /// One month
        /// </summary>
        M, 
        /// <summary>
        /// One Year
        /// </summary>
        Y, 
        /// <summary>
        /// Just all
        /// </summary>
        A
    }
}
