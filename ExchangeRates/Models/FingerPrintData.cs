using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace ExchangeRates.Models
{
    public class FingerPrintData
    {
        [Key]
        public string Id { get; set; }

        public FingerPrintData() { }

        public FingerPrintData(object data)
        {
            var dt = JsonSerializer.Deserialize<FingerPrintData>(JsonSerializer.Serialize(data));
            c = dt?.c;
            d = dt?.d;
            i = dt?.i;
        }

        /// <summary>
        /// Counter
        /// </summary>
        public int? c { get; set; }
        /// <summary>
        /// Last visit date
        /// </summary>
        public DateTime? d { get; set; }
        /// <summary>
        /// Initial date
        /// </summary>
        public DateTime? i { get; set; }
        public VisitorDataItem VisitorDataItem { get; set; }
        public string VisitorDataItemId { get; set; }
    }
}
