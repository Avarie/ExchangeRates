using System.Collections.Generic;
using CurrencyAggregator.Models;

namespace CurrencyAggregator.Sources
{
    public interface ICurrencyDataSource
    {
        List<CurrencyItem> GetSource();
    }
}
