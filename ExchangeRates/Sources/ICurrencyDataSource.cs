using System.Collections.Generic;
using ExchangeRates.Models;

namespace ExchangeRates.Sources
{
    public interface ICurrencyDataSource
    {
        List<CurrencyItem> GetSource();
    }
}
