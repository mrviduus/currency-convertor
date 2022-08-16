using CurrenyConvertor.Configuration.Options;
using CurrenyConvertor.Model;
using CurrenyConvertor.Model.Enums;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace CurrenyConvertor.Services
{
    public class CurrenyConvertorService : ICurrenyConvertorService
    {
        private readonly CurrenyConvertorOptions config;
        
        public CurrenyConvertorService(IOptionsSnapshot<CurrenyConvertorOptions> options)
        {

            if (string.IsNullOrEmpty(options.Value.ApiBaseUrl))
            {
                throw new ArgumentException("ApiBaseUrl is null of empty", nameof(options.Value.ApiBaseUrl));
            }

            config = options.Value;
        }

        public async Task<IEnumerable<CurrenyConvertorResponse>> ExchangeRateAsync(
            double amount,
            CurrencyType from,
            CurrencyType to,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {

            string seriesNames = CurrenyConvertorConstants.SeriesNames
                .Replace("{from}", from.ToString())
                .Replace("{to}", to.ToString());

            var url = CreateQuery(seriesNames, startDate, endDate);

            IFlurlResponse response;

            try
            {
                response = await url.GetAsync();

                return ParseJsonResponse(
                    amount,
                    await response.ResponseMessage.Content.ReadAsStringAsync(),
                    seriesNames);
            }
            catch (FlurlHttpException ex) when (ex.StatusCode == 404)
            {
                throw new Exception("HttpStatusCode.NotFound");
            }
            catch (FlurlHttpException ex) when (ex.StatusCode == 400)
            {
                throw new Exception("HttpStatusCode.BadRequest");
            }

        }

        private string CreateQuery(string seriesNames, DateTime? startDate, DateTime? endDate)
        {
            var url = CurrenyConvertorConstants.BankOfCanadaQuery
                      .Replace("{ApiBaseUrl}", config.ApiBaseUrl)
                      .Replace("{SeriesNames}", seriesNames);

            if (startDate.HasValue)
            {
                url = url.SetQueryParam("start_date", startDate.Value.ToString("yyyy-MM-dd"));
            }

            if (endDate.HasValue)
            {
                url = url.SetQueryParam("end_date", endDate.Value.ToString("yyyy-MM-dd"));
            }
            
            if (!startDate.HasValue && !endDate.HasValue)
            {
                url = url.AppendPathSegment("json");
                url = url.SetQueryParam("recent", 1);
            }

            return url;
        }

        private IEnumerable<CurrenyConvertorResponse> ParseJsonResponse(double amount, string contentString, string seriesNames)
        {
            JObject jobject = JObject.Parse(contentString);

            var results = jobject["observations"].ToList();

            foreach (var item in results)
            {
                string date = item.Value<string>("d");
                var seriesValue = item.Value<JToken>(string.Format(seriesNames));
                double value = Convert.ToDouble(seriesValue.Value<string>("v"));

                yield return new(date, СalculateExchangeRate(amount, value));
            };
        }

        private double СalculateExchangeRate(double amount, double value) => Math.Round((amount * value), CurrenyConvertorConstants.AfterDecimalPoint);

    }
}
