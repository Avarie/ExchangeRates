using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CurrencyAggregator.Models
{
    public class VisitorDataItem
    {
        public VisitorDataItem() {
            Id = Guid.NewGuid().ToString();
            FingerPrints = new List<FingerPrintData>();
        }

        [Key]
        public string Id { get; set; }

        /// <summary>
        /// UI finger print
        /// </summary>
        public ICollection<FingerPrintData> FingerPrints { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastVisit { get; set; }
        public int? Count { get; set; }
        public string Ip { get; set; }
        public void ActualizeData()
        {
            LastVisit = FingerPrints.Max(x => x.d);
            Count = FingerPrints.Sum(x => x.c);
        }
    }
}