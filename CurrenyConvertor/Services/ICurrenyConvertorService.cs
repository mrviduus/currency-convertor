using CurrenyConvertor.Model;
using CurrenyConvertor.Model.Enums;

namespace CurrenyConvertor.Services
{
    public interface ICurrenyConvertorService
    {
        /// <summary>
        /// Сalculates the exchange rate for the specified period.
        /// </summary>
        /// <param name="from">CurrencyType</param>
        /// <param name="to">CurrencyType</param>
        /// <param name="startDate">Optional to get data for the specified period (example 2022-08-09)</param>
        /// <param name="endDate">Optional to get data for the specified period (example 2022-08-09)</param>
        /// <returns>List of exchange rates</returns>
        Task<IEnumerable<CurrenyConvertorResponse>> ExchangeRateAsync(
            double amount,
            CurrencyType from,
            CurrencyType to,
            DateTime? startDate = null,
            DateTime? endDate = null);
    }
}
