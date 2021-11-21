using System;
using System.ComponentModel.DataAnnotations;

namespace CurrencyAggregator.Models
{
    public class CurrencyItem
    {
        public CurrencyItem()
        {
            Id = Guid.NewGuid().ToString();
            Date = DateTime.Now;
        }

        public CurrencyItem(string name) : this()
        {
            Name = name;
        }

        [Key]
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }

        public string Name { get; set; }
        public decimal Buy { get; set; }
        public decimal Sell { get; set; }


        public decimal Diff => Sell > 0 && Buy > 0 ? Sell - Buy : 0;
    }
}