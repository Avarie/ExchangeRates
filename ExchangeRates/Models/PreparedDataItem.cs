using System;
using System.ComponentModel.DataAnnotations;

namespace ExchangeRates.Models
{
    public class PreparedDataItem
    {
        public PreparedDataItem() {
            Id = Guid.NewGuid().ToString();
        }

        [Key]
        public string Id { get; set; }

        public PreparedDataItem(string key, string data) : this()
        {
            Key = key;
            Data = data;
        }

        public string Key { get; set; }
        public string Data { get; set; }
    }

    public class PreparedDataItemKeys
    {
        public static string VolatilityLabels => "labels";
        public static string VolatilityDataSets => "datasets";
        public static string VolatilityPercents => "percents";
    }
}