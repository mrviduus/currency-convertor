using CurrenyConvertor.Configuration.Options;
using CurrenyConvertor.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CurrenyConvertor.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services,
            IConfiguration configuration) =>
            services.AddScoped<ICurrenyConvertorService, CurrenyConvertorService>()
            .Configure<CurrenyConvertorOptions>(
            x => configuration.GetSection("CurrenyConvertor").Bind(x));
    }
}
