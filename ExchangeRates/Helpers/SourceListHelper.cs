using ExchangeRates.Resources;

namespace ExchangeRates.Helpers
{
    public static class SourceListHelper
    {
        public const string MonoBank = "MonoBank";
        public const string NBU = "NBU";
        public const string PrivatBankCash = "PrivatBank Cash";
        public const string PrivatBankCashLess = "PrivatBank CashLess";
        public const string ObmenkaRetail = "Obmenka Retail";
        public const string ObmenkaWholesale = "Obmenka Wholesale";
        public const string InvestingCom = "Investing.Com";

        public static string GetTitle(string dbName) => dbName switch
        {
            NBU => Str.SourceTitle_NBU,
            PrivatBankCash => Str.SourceTitle_PrivatBankCash,
            PrivatBankCashLess => Str.SourceTitle_PrivatBankCashLess,
            ObmenkaRetail => Str.SourceTitle_ObmenkaRetail,
            ObmenkaWholesale => Str.SourceTitle_ObmenkaWholesale,
            InvestingCom => Str.SourceTitle_InvestingCom,
            MonoBank => Str.SourceTitle_MonoBank
        };
    }
}