namespace CurrenyConvertor
{
    public class CurrenyConvertorConstants
    {
        public const string BankOfCanadaQuery = "{ApiBaseUrl}/valet/observations/{SeriesNames}";

        public const string SeriesNames = "FX{from}{to}";

        public const int AfterDecimalPoint = 4;
    }
}
